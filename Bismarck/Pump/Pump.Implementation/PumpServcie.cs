using Cmn.Constants;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Pump.Domain;
using Rincon.Bus;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Rincon.Bus.Messaging;

namespace Pump.Implementation
{
    /// <inheritdoc />
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class PumpServcie : StatelessService
    {
        public PumpServcie(StatelessServiceContext context)
            : base(context)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context => new SqlBusCommunicationListener(new SqlBusOptions
                {
                    AppQueueName = BismarckAppQueue.PumpQueue,
                    ErrorQueueName = BismarckAppQueue.ErrorQueue,
                    SqlConnection = BismarckConsts.SqlServerConnection,
                    RegisterHandlers = RegisterHandlers
                }))
            };
        }


        private void RegisterHandlers(IMessageProcessor _, CancellationToken token)
        {
            _.Register(handlerBuilder => handlerBuilder.Build<PumpCalculateOneCommandHandler>(token));
            //_.Register(handlerBuilder => handlerBuilder.Build<PumpCalculateOneCommandHandler>(token));
            //_.Register(handlerBuilder => handlerBuilder.Build<PumpCalculateOneCommandHandler>(token));
            //_.Register(handlerBuilder => handlerBuilder.Build<PumpCalculateOneCommandHandler>(token));
            //_.Register(handlerBuilder => handlerBuilder.Build<PumpCalculateOneCommandHandler>(token));
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
