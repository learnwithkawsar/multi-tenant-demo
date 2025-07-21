using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.Constants;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Interfaces;
using MultiTenantDemo.Models;

namespace MultiTenantDemo.Implementation
{
    internal class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(TenantDbContext tenantDbContext, IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger, ApplicationDbContext dbContext)
        {
            _tenantDbContext = tenantDbContext;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
        {
            await InitializeTenantDbAsync(cancellationToken);

            foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
            {
                await InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
            }

            _logger.LogInformation("For documentations and guides, visit https://www.fullstackhero.net");
            _logger.LogInformation("To Sponsor this project, visit https://opencollective.com/fullstackhero");
        }

        public async Task InitializeApplicationDbForTenantAsync(AppTenantInfo tenant, CancellationToken cancellationToken)
        {
            if (tenant?.ConnectionString == null)
            {
                _logger.LogError($"Invalid tenant or connection string for tenant: {tenant?.Id}");
                throw new ArgumentException("Tenant must have a valid connection string");
            }

            _logger.LogInformation($"Initializing database for tenant: {tenant.Name} ({tenant.Id})");
            _logger.LogInformation($"Using connection: {tenant.ConnectionString}");

            // Create DbContext options directly with tenant's connection string
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(tenant.ConnectionString); // or UseSqlServer

            // Create a mock accessor for the tenant context
            var mockAccessor = new MockMultiTenantContextAccessor(tenant);

            try
            {
                // Create DbContext with specific connection
                using var dbContext = new ApplicationDbContext(mockAccessor, optionsBuilder.Options);

                _logger.LogInformation($"DbContext created with connection: {dbContext.Database.GetConnectionString()}");

                // Test connection first
                if (!await dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogError($"Cannot connect to database for tenant {tenant.Id}");
                    return;
                }

                // Apply migrations
                if (dbContext.Database.GetMigrations().Any())
                {
                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                    if (pendingMigrations.Any())
                    {
                        _logger.LogInformation($"Applying {pendingMigrations.Count()} migrations for tenant '{tenant.Id}'");
                        await dbContext.Database.MigrateAsync(cancellationToken);
                        _logger.LogInformation($"Successfully applied migrations for tenant '{tenant.Id}'");
                    }
                    else
                    {
                        _logger.LogInformation($"No pending migrations for tenant '{tenant.Id}'");
                    }
                }

                _logger.LogInformation($"Database initialization completed for tenant '{tenant.Id}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to initialize database for tenant '{tenant.Id}': {ex.Message}");
                throw;
            }
        }

        // Helper class for creating a mock accessor


        private async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
        {
            if (_tenantDbContext.Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Applying Root Migrations.");
                await _tenantDbContext.Database.MigrateAsync(cancellationToken);
            }

            await SeedRootTenantAsync(cancellationToken);
        }

        private async Task SeedRootTenantAsync(CancellationToken cancellationToken)
        {
            if (await _tenantDbContext.TenantInfo.FindAsync(new object?[] { MultitenancyConstants.Root.Id }, cancellationToken: cancellationToken) is null)
            {
                var rootTenant = new AppTenantInfo(
                    MultitenancyConstants.Root.Id,
                    MultitenancyConstants.Root.Name,
                    string.Empty,
                    MultitenancyConstants.Root.EmailAddress);

                rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));

                _tenantDbContext.TenantInfo.Add(rootTenant);

                await _tenantDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class MockMultiTenantContextAccessor : IMultiTenantContextAccessor
    {
        public IMultiTenantContext MultiTenantContext { get; } // Note: IMultiTenantContext interface

        public MockMultiTenantContextAccessor(AppTenantInfo tenant)
        {
            MultiTenantContext = new MultiTenantContext<AppTenantInfo>
            {
                TenantInfo = tenant
            };
        }
    }
}
