using System;

namespace Rincon.Core.Messaging
{
    public class IntegrationMessage : IIntegrationMessage
    {
        public Guid CorrelationUuid { get; set; }
        public Guid UserUuid { get; set; }
    }
}
