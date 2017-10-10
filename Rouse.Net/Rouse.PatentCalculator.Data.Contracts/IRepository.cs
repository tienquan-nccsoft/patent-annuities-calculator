using System.Linq;

namespace Rouse.PatentCalculator.Data.Contracts
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        TEntity GetByID(object id);

        IQueryable<TEntity> GetAll();

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        bool Delete(object id);

        bool SoftDelete(object id);
    }
}
