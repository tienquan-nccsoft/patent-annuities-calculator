using Rouse.PatentCalculator.DomainModels;
using System.Data.Entity;

namespace Rouse.PatentCalculator.Data
{
    public class TaxRepository: Repository<Tax>
    {
        public TaxRepository(PatCalculatorEntities context) : base(context)
        {
        }

        public override bool SoftDelete(object id)
        {
            var entity = dbSet.Find(id);
            entity.Deleted = true;
            context.Entry(entity).State = EntityState.Modified;
            return context.SaveChanges() > 0;
        }
        public override Tax Add(Tax entity)
        {
            if(dbSet.AnyAsync(s => s.CountryCode == entity.CountryCode && !s.Deleted).Result) {
                return null;
            }
            return base.Add(entity);
        }

        public override Tax Update(Tax entity)
        {
            try
            {
                return base.Update(entity);
            }
            catch (System.Exception ex)
            {
                return null;
                throw;
            }
        }
    }
}
