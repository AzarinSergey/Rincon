using System;
using System.ComponentModel.DataAnnotations;
using Cmn.Enums;
using Rincon.EntityFramwork;
using Rincon.Repository;

namespace Pump.Model.Entity
{
    public class PumpCalculation : IAggregationRoot
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        [Required]
        public Guid? CalcUuid { get; set; }

        public PumpCalcState CalcState { get; set; }

        [Required]
        public decimal? Temperature { get; set; }

        [Required]
        public decimal? WallTemperature { get; set; }

        [Required]
        public decimal? Pressure { get; set; }

        public virtual CalculationRequestInfo CalcRequestInfo { get; set; }
    }
}
