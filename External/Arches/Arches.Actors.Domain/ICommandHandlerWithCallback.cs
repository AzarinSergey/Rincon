using Arches.Contract;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arches.Actors.Domain
{
    public interface ICommandHandlerWithCallback
    {
        Task SuccessCallback(ICalcCommandWithCallback command, HttpClient client);
        Task ErrorCallback(ICalcCommandWithCallback command, HttpClient client);
    }
}