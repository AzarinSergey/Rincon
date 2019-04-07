using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Rincon.Repository;
using System.Data;
using System.Threading;

namespace Rincon.EntityFramwork
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly RinconDbContext _dbContext;
        private readonly CancellationToken _token;

        private readonly IDbContextTransaction _transaction;
        private readonly bool _withinTransaction;

        public EntityFrameworkUnitOfWork(IRinconDbContext dbContext, IsolationLevel? isolationLevel, CancellationToken token)
        {
            _dbContext = (RinconDbContext)dbContext;
            _token = token;
            _withinTransaction = isolationLevel.HasValue;

            if (isolationLevel.HasValue)
            {
                _transaction = _dbContext.Database.BeginTransaction(isolationLevel.Value);
            }
        }

        public ICommandRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IAggregationRoot
        {
            return new EntityFrameworkCommandRepository<TEntity>(_dbContext, _withinTransaction, _token);
        }

        void IUnitOfWork.Commit()
        {
            _transaction?.Commit();
        }

        void IUnitOfWork.Rollback()
        {
            _transaction?.Rollback();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
            _transaction?.Dispose();
        }
    }
}