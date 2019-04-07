using Rincon.Core.Messaging;

namespace Pump.Contract
{
    public class PumpCalculateOneIntegrationCommand : IntegrationMessage
    {
        public string VxDataUrl { get; set; }
        public string EcnDataUrl { get; set; }
        public decimal Temperature { get; set; }
        public decimal WallTemperature { get; set; }
        public decimal Pressure { get; set; }
    }
}
