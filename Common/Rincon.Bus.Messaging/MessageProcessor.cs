using Rebus.Activation;
using Rincon.Core;
using Rincon.Core.Domain;
using Rincon.Core.Messaging;
using Rincon.Repository;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Rincon.Bus.Messaging
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly Func<IsolationLevel?, IUnitOfWork> _getUow;
        private readonly BuiltinHandlerActivator _activator;

        public MessageProcessor(Func<IsolationLevel?, IUnitOfWork> getUow, BuiltinHandlerActivator activator)
        {
            _getUow = getUow;
            _activator = activator;
        }

        public void Dispose()
        {
            _activator?.Dispose();
        }

        public Task Register<TCommand>(Func<HandlerBuilder, CommandHandler<TCommand>> func) 
            where TCommand : IntegrationMessage
        {
            _activator.Handle<TCommand>(async c =>
            {
                var builder = new HandlerBuilder(_getUow);
                var handler = func(builder);
                try
                {
                    await handler.Process(c);
                }
                catch (Exception)
                {
                    handler.Uow.Rollback();
                }
            });

            return Task.CompletedTask;
        }
    }
}