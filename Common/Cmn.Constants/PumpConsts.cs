namespace Cmn.Constants
{
    /// <summary>
    /// Общие костанты. Расшарил так для простоты решения.
    /// Иначе нужно дублировать настройки для внешних сервисов (Arches) и сервиса Pump кластера (Bismarck)
    /// </summary>
    public class PumpConsts
    {
        public const string MinioBucketName = "pump";
        public const string EcnFilePrefix = "ecn_vr";
        public const string VxFilePrefix = "vx";
        public const string DataFileExtension = ".csv";
        public const string OutputFileExtension = ".tsv";
        public const string OutputFilePrefix = "output";
    }
}
