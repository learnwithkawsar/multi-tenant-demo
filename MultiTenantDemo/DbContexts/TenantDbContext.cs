using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.Constants;
using MultiTenantDemo.Models;

namespace MultiTenantDemo.DbContexts
{
    public class TenantDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
            // AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
        }
    }
}
