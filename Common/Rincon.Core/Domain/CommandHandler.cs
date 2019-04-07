using Rincon.Core.Messaging;
using Rincon.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace Rincon.Core.Domain
{
    public abstract class CommandHandler
    {
        public CancellationToken Token { get; internal set; }

        public IUnitOfWork Uow { get; internal set; }
    }

    public abstract class CommandHandler<TCommand> : CommandHandler
        where TCommand : IntegrationMessage
    {
        public abstract Task Process(TCommand command);
    }
}