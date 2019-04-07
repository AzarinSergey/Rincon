using Microsoft.EntityFrameworkCore;
using Rincon.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rincon.EntityFramwork
{
    public class EntityFrameworkCommandRepository<TEntity> : ICommandRepository<TEntity> 
        where TEntity : class, IAggregationRoot
    {
        private readonly RinconDbContext _dbContext;
        private readonly bool _withinTransaction;
        private readonly CancellationToken _token;

        public EntityFrameworkCommandRepository(RinconDbContext dbContext, bool withinTransaction, CancellationToken token)
        {
            _dbContext = dbContext;
            _withinTransaction = withinTransaction;
            _token = token;
        }

        public IQueryable<TEntity> Query()
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            return _withinTransaction ? query : query.AsNoTracking();
        }

        public async Task<int> CreateOrUpdate(TEntity entity)
        {
            if (entity.Id == default(int))
            {
                _dbContext.Set<TEntity>().Add(entity);
            }
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public Task Create(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            return _dbContext.SaveChangesAsync(_token);
        }

        public Task Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return _dbContext.SaveChangesAsync(_token);
        }

        public async Task<TEntity> ApplyDetached(TEntity entity)
        {
            SetEntityState(entity);

            await _dbContext.SaveChangesAsync(_token);

            return entity;
        }

        private void SetEntityState(TEntity entity)
        {
            var currentState = _dbContext.Entry(entity).State;

            _dbContext.Entry(entity).State = entity.Id == default(int)
                ? currentState != EntityState.Added 
                    ? EntityState.Added 
                    : currentState
                : currentState != EntityState.Modified
                    ? EntityState.Modified
                    : currentState;
        }
    }
}