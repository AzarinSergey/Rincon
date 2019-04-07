using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Retry.Simple;
using Rebus.SqlServer;
using Rincon.Bus.Messaging;
using Rincon.Core;

namespace Rincon.Bus
{
    public class SqlBusCommunicationListener : ICommunicationListener
    {
        private readonly SqlBusOptions _options;
        private BuiltinHandlerActivator _activator;

        public SqlBusCommunicationListener(SqlBusOptions options)
        {
            _options = options;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            _activator = new BuiltinHandlerActivator();
            _options.RegisterHandlers(new MessageProcessor(_activator), cancellationToken);

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
