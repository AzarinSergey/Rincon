using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Arches.Actors.Domain;
using Arches.Contract;
using Arches.Contract.PumpService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Arches.Actors.Implementation
{
    public class PumpCalcActor : ReceiveActor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancel;
        private IActorRef _self;

        public PumpCalcActor(IHttpClientFactory clientFactory, ILogger logger)
        {
            _self = Self;

            _httpClientFactory = clientFactory;
            _logger = logger;
            _cancel = new CancellationTokenSource();

            ReceiveAsync<PumpCalcCommand>(command => Handler(command, new PumpCalcCommandHandler(logger)));
        }

        private async Task Handler(PumpCalcCommand command, ICalcCommandHandlerWithCallback<PumpCalcCommand> handler)
        {
            var sw = new Stopwatch();
            sw.Start();
            _logger.LogInformation($"[START] [{Self.Path}].Received Command at {DateTime.Now}, with payload: {JsonConvert.SerializeObject(command)}");

            try
            {
                await handler.Calculate(command, _cancel.Token);
                await handler.SuccessCallback(command, _httpClientFactory.CreateClient());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Handler exception.");
                await handler.ErrorCallback(command, _httpClientFactory.CreateClient());
            }
            finally
            {
                _logger.LogInformation($"[END] [{Self.Path}].Handled Command at {DateTime.Now}, total seconds - {sw.Elapsed.TotalSeconds}");
                Context.Stop(_self);
            }
        }
    }
}
