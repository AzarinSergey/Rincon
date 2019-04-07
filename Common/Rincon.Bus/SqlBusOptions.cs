using System;
using System.Threading;
using Rebus.Bus;
using Rincon.Bus.Messaging;

namespace Rincon.Bus
{
    public class SqlBusOptions
    {
        public string SqlConnection { get; set; }

        public string AppQueueName { get; set; }

        public string ErrorQueueName { get; set; }

        public Action<IBus> Subscriptions { get; set; }

        public Action<IMessageProcessor, CancellationToken> RegisterHandlers { get; set; }
    }
}