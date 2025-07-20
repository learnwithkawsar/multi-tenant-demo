using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.DbContexts;

namespace MultiTenantDemo.Implementation
{
    public class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenantInfo _currentTenant;

        private readonly ILogger<ApplicationDbInitializer> _logger;

        public ApplicationDbInitializer(ApplicationDbContext dbContext, ITenantInfo currentTenant, ILogger<ApplicationDbInitializer> logger)
        {
            _dbContext = dbContext;
            _currentTenant = currentTenant;
            // _dbSeeder = dbSeeder;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.GetMigrations().Any())
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    _logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", _currentTenant.Id);
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (await _dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", _currentTenant.Id);

                    //  await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
                }
            }
        }
    }
}
