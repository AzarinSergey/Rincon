using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Retry.Simple;
using Rebus.SqlServer;
using Rincon.Bus.Messaging;
using Rincon.EntityFramwork;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Rincon.Bus
{
    public class SqlBusCommunicationListener : ICommunicationListener
    {
        private readonly SqlBusOptions _options;
        private readonly Func<IRinconDbContext> _getDbContext;
        private BuiltinHandlerActivator _activator;

        public SqlBusCommunicationListener(
            SqlBusOptions options,
            Func<IRinconDbContext> getDbContext
            )
        {
            _options = options;
            _getDbContext = getDbContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            _activator = new BuiltinHandlerActivator();

            using (var processor = new MessageProcessor(
                isolationLevel => new EntityFrameworkUnitOfWork(_getDbContext(), isolationLevel, cancellationToken),
                _activator))
            {
                _options.RegisterHandlers(processor, cancellationToken);

                var bus = Configure.With(_activator)
                    .Logging(x => x.ColoredConsole(LogLevel.Info))
                    .Transport(t => t.UseSqlServer(() =>
                            new DbConnectionProvider(_options.SqlConnection,
                                new ConsoleLoggerFactory(true)).GetConnection(),
                        _options.AppQueueName))
                    .Options(o => o.SimpleRetryStrategy(_options.ErrorQueueName))
                    .Start();

                _options.Subscriptions?.Invoke(bus);

                return Task.FromResult(_options.SqlConnection);
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _activator?.Dispose();

            return Task.CompletedTask;
        }

        public void Abort()
        {
            _activator?.Dispose();
        }
    }
}
