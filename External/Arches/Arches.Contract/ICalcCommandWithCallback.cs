namespace Arches.Contract
{
    public interface ICalcCommandWithCallback : ICalcCommand
    {
        string SuccessCallbackUrl { get; set; }

        string ErrorCallbackUrl { get; set; }
    }
}