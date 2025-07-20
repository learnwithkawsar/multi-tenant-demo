using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantDemo.Models;

namespace MultiTenantDemo.DbContexts
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(ITenantInfo currentTenant, DbContextOptions options) : base(currentTenant, options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }


    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.IsMultiTenant();

            builder
                .Property(b => b.Name)
                    .HasMaxLength(1024);
        }
    }
}
