using System;

namespace Rincon.Core.Messaging
{
    public interface IIntegrationMessage
    {
        Guid CorrelationUuid { get; set; }
        Guid UserUuid { get; set; }
    }
}