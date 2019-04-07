using System;
using System.Threading.Tasks;
using Pump.Contract;
using Rincon.Core.Domain;

namespace Pump.Domain
{
    public class PumpCalculateOneCommandHandler : CommandHandler<PumpCalculateOneIntegrationCommand>
    {
        public override Task Process(PumpCalculateOneIntegrationCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
