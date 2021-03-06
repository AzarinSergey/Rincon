﻿using Cmn.Constants;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Pump.Domain;
using Rincon.Bus;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pump.Model;
using Rincon.Bus.Messaging;
using Rincon.EntityFramwork;

namespace Pump.Implementation
{
    /// <inheritdoc />
    internal sealed class PumpServcie : StatelessService
    {
        public PumpServcie(StatelessServiceContext context)
            : base(context)
        { }

        /// <inheritdoc />
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context => new SqlBusCommunicationListener(
                        new SqlBusOptions
                        {
                            AppQueueName = BismarckAppQueue.PumpQueue,
                            ErrorQueueName = BismarckAppQueue.ErrorQueue,
                            SqlConnection = BismarckConsts.SqlServerConnection,
                            RegisterHandlers = RegisterHandlers
                        },
                        () => new PumpDbContext()
                    )
                )
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
            new PumpDbContext()
                .Database.Migrate();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
