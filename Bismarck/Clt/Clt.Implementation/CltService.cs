using Clt.Model;
using Cmn.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Rincon.Bus;
using Rincon.Bus.Messaging;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Clt.Implementation
{
    /// <inheritdoc />
    internal sealed class CltService : StatelessService
    {
        public CltService(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context => new SqlBusCommunicationListener(
                        new SqlBusOptions
                        {
                            AppQueueName = BismarckAppQueue.CltQueue,
                            ErrorQueueName = BismarckAppQueue.ErrorQueue,
                            SqlConnection = BismarckConsts.SqlServerConnection,
                            RegisterHandlers = RegisterHandlers
                        },
                        () => new CltDbContext()
                    )
                )
            };
        }

        private void RegisterHandlers(IMessageProcessor _, CancellationToken token)
        { }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            new CltDbContext()
                .Database.Migrate();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
