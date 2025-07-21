using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.DbContexts;

namespace MultiTenantDemo.Implementation
{
    public class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApplicationDbInitializer> _logger;

        public ApplicationDbInitializer(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger<ApplicationDbInitializer> logger)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var tenantInfo = _serviceProvider.GetService<ITenantInfo>();
            if (tenantInfo == null)
            {
                _logger.LogWarning("TenantInfo is not available in this context.");
                return;
            }

            if (_dbContext.Database.GetMigrations().Any())
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    _logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", tenantInfo.Id);
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (await _dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", tenantInfo.Id);

                    //  await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
                }
            }
        }
    }
}
