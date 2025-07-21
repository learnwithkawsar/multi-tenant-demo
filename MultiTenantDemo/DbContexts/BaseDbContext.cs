namespace MultiTenantDemo.DbContexts
{
    //public class BaseDbContext : MultiTenantIdentityDbContext<ApplicationUser>
    //{
    //    public BaseDbContext(ITenantInfo currentTenant, DbContextOptions options) : base(currentTenant, options)
    //    {
    //    }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        // TODO: We want this only for development probably... maybe better make it configurable in logger.json config?
    //        optionsBuilder.EnableSensitiveDataLogging();

    //        // If you want to see the sql queries that efcore executes:

    //        // Uncomment the next line to see them in the output window of visual studio
    //        // optionsBuilder.LogTo(m => System.Diagnostics.Debug.WriteLine(m), Microsoft.Extensions.Logging.LogLevel.Information);

    //        // Or uncomment the next line if you want to see them in the console
    //        // optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

    //        if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
    //        {
    //            optionsBuilder.UseNpgsql(TenantInfo.ConnectionString);
    //        }
    //    }
    //}
}
