using Rouse.PatentCalculator.AzureAD.DomainModels;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rouse.PatentCalculator.AzureAD.Data
{
    public class TokenCacheDataContext : DbContext
    {
        public TokenCacheDataContext()
                : base("UserCacheConnectionString") { }

        public DbSet<PerUserWebCache> PerUserCacheList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
