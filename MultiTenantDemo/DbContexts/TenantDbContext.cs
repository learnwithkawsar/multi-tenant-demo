using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.Constants;
using MultiTenantDemo.Models;

namespace MultiTenantDemo.DbContexts
{
    public class TenantDbContext : EFCoreStoreDbContext<FSHTenantInfo>
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FSHTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
        }
    }
}
