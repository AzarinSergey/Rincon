using Arches.Contract;
using Microsoft.Extensions.Logging;

using Minio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arches.Contract.PumpService;
using Cmn.Constants;
using Microsoft.AspNetCore.WebUtilities;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

// ReSharper disable InconsistentNaming

namespace Arches.Actors.Domain
{
    /// <summary>
    /// Тут прототип логики запуска рассчета и отправки сообщений с отчетами.
    /// </summary>
    public class PumpCalcCommandHandler : ICalcCommandHandlerWithCallback<PumpCalcCommand>
    {
        private const string CalcTemplateCmdFilesPath = "ServiceStaticFiles\\PumpService\\Template";
        private const string CalcArtifactsPath = "ServiceStaticFiles\\PumpService\\Artifacts";
        private const string InputPyTemplateFileName = "input.py";
        private const string ExeFileName = "PyHyCarSim.exe";

        private readonly ILogger _logger;
        private string _calcDirectoryPath;
        private Process _cmd;

        public PumpCalcCommandHandler(ILogger logger)
        {
            _logger = logger;
        }

        private async Task SendCallback(string calculationUuid, string callbackUrl, HttpClient client)
        {
            var url = QueryHelpers.AddQueryString(callbackUrl, new Dictionary<string, string>
            {
                {nameof(calculationUuid), calculationUuid}
            });

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            _logger.LogInformation(response.IsSuccessStatusCode
                ? $"[{response.StatusCode}] Callback succefully delivered. URL: {url}"
                : $"[{response.StatusCode}] Callback not delivered. URL: {url}");
        }

        public Task SuccessCallback(ICalcCommandWithCallback command, HttpClient client)
            => SendCallback(command.CalculationUuid, command.SuccessCallbackUrl, client);

        public Task ErrorCallback(ICalcCommandWithCallback command, HttpClient client)
            => SendCallback(command.CalculationUuid, command.ErrorCallbackUrl, client);

        public async Task Calculate(PumpCalcCommand command, CancellationToken cancelToken)
        {
            var calcUuid = command.CalculationUuid.ToLower();
            var minioClient = new MinioClient(BismarckConsts.MinioServerHost, BismarckConsts.MinioServerAccessKey, BismarckConsts.MinioServerSecretKey);

            var dataInput = await GetInputDataFromBucket(minioClient, calcUuid, cancelToken);
            var vxString = dataInput.Item1;
            var ecnString = dataInput.Item2;

            var runtimeDirectory = Directory.GetCurrentDirectory();
            var copyFrom = Path.Combine(runtimeDirectory, CalcTemplateCmdFilesPath);
            var copyTo = Path.Combine(runtimeDirectory, CalcArtifactsPath, calcUuid);
            _calcDirectoryPath = CopyFilesToNewDirectory(copyFrom, copyTo);

            var inputPyFilePath = Path.Combine(_calcDirectoryPath, InputPyTemplateFileName);
            var VxFilePath = Path.Combine(_calcDirectoryPath, PumpConsts.VxFilePrefix) + PumpConsts.DataFileExtension;
            var EcnFilePath = Path.Combine(_calcDirectoryPath, PumpConsts.EcnFilePrefix) + PumpConsts.DataFileExtension;

            var filledInputPy = FillInputPy(
                    new StubbleBuilder().Build(),
                    File.ReadAllText(Path.Combine(copyFrom, InputPyTemplateFileName), Encoding.UTF8),
                    new Dictionary<string, string>
                    {
                        { nameof(command.Pressure), command.Pressure.ToString(CultureInfo.InvariantCulture) },
                        { nameof(command.Temperature), command.Temperature.ToString(CultureInfo.InvariantCulture)},
                        { nameof(command.WallTemperature), command.WallTemperature.ToString(CultureInfo.InvariantCulture)},
                        { nameof(VxFilePath), VxFilePath},
                        { nameof(EcnFilePath), EcnFilePath}
                    }
                );

            using (var inputPyStream = File.Create(inputPyFilePath))
            {
                await inputPyStream.WriteAsync(Encoding.UTF8.GetBytes(filledInputPy), cancelToken);
            }

            using (var ecnFs = File.Create(EcnFilePath))
            {
                await ecnFs.WriteAsync(Encoding.UTF8.GetBytes(ecnString), cancelToken);
            }

            using (var vxFs = File.Create(VxFilePath))
            {
                await vxFs.WriteAsync(Encoding.UTF8.GetBytes(vxString), cancelToken);
            }

            RunCmd(_calcDirectoryPath);

            var outputFileName = PumpConsts.OutputFilePrefix + calcUuid + PumpConsts.OutputFileExtension;

            var bs = File.ReadAllBytes(Path.Combine(_calcDirectoryPath, PumpConsts.OutputFilePrefix) + PumpConsts.OutputFileExtension);
            using (var filestream = new MemoryStream(bs))
            {
                await minioClient.PutObjectAsync(PumpConsts.MinioBucketName, outputFileName, filestream, filestream.Length, cancellationToken: cancelToken);
            }

            Directory.Delete(_calcDirectoryPath, true);
        }

        public string FillInputPy(StubbleVisitorRenderer renderer, string v, Dictionary<string, string> dictionary)
        {
            return renderer.Render(v, dictionary, new RenderSettings
            {
                SkipHtmlEncoding = true,
                SkipRecursiveLookup = false,
                ThrowOnDataMiss = true
            });
        }

        private async Task<(string, string)> GetInputDataFromBucket(MinioClient client, string calcUuid, CancellationToken cancelToken)
        {
            var vxString = string.Empty;
            var ecnString = string.Empty;

            var vxFileNameInBucket = PumpConsts.VxFilePrefix + calcUuid + PumpConsts.DataFileExtension;
            var ecnFileNameInBucket = PumpConsts.EcnFilePrefix + calcUuid + PumpConsts.DataFileExtension;

            await Task.WhenAll(
                client.GetObjectAsync(PumpConsts.MinioBucketName, vxFileNameInBucket, x =>
                {
                    vxString = GetString(x);
                }, cancellationToken: cancelToken),
                client.GetObjectAsync(PumpConsts.MinioBucketName, ecnFileNameInBucket, x =>
                {
                    ecnString = GetString(x);
                }, cancellationToken: cancelToken)
            );

            return (vxString, ecnString);
        }

        private void RunCmd(string calcDirectoryPath)
        {
            _cmd = new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = Path.Combine(calcDirectoryPath, ExeFileName),
                    WorkingDirectory = calcDirectoryPath,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas"
                }
            };

            _cmd.Start();
            _cmd.WaitForExit();

            Console.WriteLine(_cmd.StandardError.ReadToEnd());

            _cmd.Close();
        }

        private string CopyFilesToNewDirectory(string sourceDir, string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                var i = 0;
                while (Directory.Exists(targetDir + "Aborted" + i))
                    i++;

                Directory.Move(targetDir, targetDir + "Aborted" + i);
            }

            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            return targetDir;
        }

        private static string GetString(Stream stream)
            => new StreamReader(stream).ReadToEnd();
    }
}
