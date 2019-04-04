namespace Arches.Contract.PumpService
{
    public class PumpCalcCommand : ICalcCommandWithCallback
    {
        public string SuccessCallbackUrl { get; set; }
        public string ErrorCallbackUrl { get; set; }
        public string CalculationUuid { get; set; }
        public string VxDataUrl { get; set; }
        public string EcnDataUrl { get; set; }
        public decimal Temperature { get; set; }
        public decimal WallTemperature { get; set; }
        public decimal Pressure { get; set; }
    }
}
