using Rouse.PatentCalculator.Data.Contracts;
using System;
using System.Data.Entity;
using System.Linq;

namespace Rouse.PatentCalculator.Data
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected DbSet<TEntity> dbSet;
        protected PatCalculatorEntities context;

        public Repository(PatCalculatorEntities context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return dbSet.AsQueryable<TEntity>();
        }       

        public virtual TEntity Add(TEntity entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            context.Entry<TEntity>(entity).State = EntityState.Modified;
            context.SaveChanges();
            return entity;
        }

        public virtual bool Delete(object id)
        {
            var entity = GetByID(id);
            context.Entry<TEntity>(entity).State = EntityState.Deleted;
            return context.SaveChanges() > 0;
        }

        public virtual bool SoftDelete(object id)
        {
            throw new NotImplementedException();
        }
    }
}
