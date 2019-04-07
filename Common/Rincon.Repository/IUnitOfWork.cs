using System;

namespace Rincon.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ICommandRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IAggregationRoot;

        void Commit();
        void Rollback();
    }
}