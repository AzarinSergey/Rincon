using Rincon.Core.Domain;
using Rincon.Repository;
using System;
using System.Data;
using System.Threading;

namespace Rincon.Core
{
    public class HandlerBuilder
    {
        private readonly Func<IsolationLevel?, IUnitOfWork> _getDbContext;

        public HandlerBuilder(Func<IsolationLevel?, IUnitOfWork> getDbContext)
        {
            _getDbContext = getDbContext;
        }

        public T Build<T>(CancellationToken token, IsolationLevel? isolationLevel = null)
            where T : CommandHandler, new()
        {
            using (var uow = _getDbContext(isolationLevel))
            {
                var result = new T
                {
                    Token = token,
                    Uow = uow
                };

                return result;
            }
        }
    }
}