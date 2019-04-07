using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rincon.Repository
{
    public interface ICommandRepository<TEntity>
    {
        IQueryable<TEntity> Query();
        Task<int> CreateOrUpdate(TEntity entity);
        Task Create(IEnumerable<TEntity> entities);
        Task Delete(TEntity entity);
        Task<TEntity> ApplyDetached(TEntity entity);
    }
}