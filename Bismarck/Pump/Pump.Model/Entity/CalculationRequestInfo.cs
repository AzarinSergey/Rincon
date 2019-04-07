using Rincon.EntityFramwork;
using Rincon.Repository;

namespace Pump.Model.Entity
{
    public class CalculationRequestInfo : IAggregationRoot
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public bool HasError { get; set; }
    }
}