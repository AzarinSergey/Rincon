using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rincon.Core;
using Rincon.Core.Domain;
using Rincon.Core.Messaging;

namespace Rincon.Bus.Messaging
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly BuiltinHandlerActivator _activator;

        //тут нужен зареганый DI логер и контекст бд
        public MessageProcessor(
                BuiltinHandlerActivator activator
            )
        {
            _activator = activator;
        }

        public void Dispose() => _activator?.Dispose();

        public Task Register<TCommand>(Func<HandlerBuilder, CommandHandler<TCommand>> func) 
            where TCommand : IntegrationMessage
        {
            _activator.Handle<TCommand>(c =>
            {
                var builder = new HandlerBuilder();

                var handler = func(builder);

                handler.Process(c);

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}