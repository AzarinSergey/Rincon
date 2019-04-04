using System.Threading;
using System.Threading.Tasks;
using Arches.Contract;
using Arches.Contract.PumpService;

namespace Arches.Actors.Domain
{
    public interface ICalcCommandHandlerWithCallback<in T> : ICommandHandlerWithCallback
        where T : ICalcCommandWithCallback
    {
        Task Calculate(T command, CancellationToken cancelToken);
       
    }
}