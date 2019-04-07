using System;
using System.Threading.Tasks;
using Rincon.Core;
using Rincon.Core.Domain;
using Rincon.Core.Messaging;

namespace Rincon.Bus.Messaging
{
    public interface IMessageProcessor : IDisposable
    {
        Task Register<TCommand>(Func<HandlerBuilder, CommandHandler<TCommand>> func) 
            where TCommand : IntegrationMessage;
    }
}