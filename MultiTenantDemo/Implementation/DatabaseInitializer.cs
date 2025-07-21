using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.Constants;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Interfaces;
using MultiTenantDemo.Models;
using System.Reflection;

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
            // First create a new scope
            //using var scope = _serviceProvider.CreateScope();

            //// //Then set current tenant so the right connectionstring is used
            //var accessor = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>();

            //// // Manually create and assign the context if it's null
            //if (accessor.MultiTenantContext is not MultiTenantContext<AppTenantInfo> context)
            //{
            //    context = new MultiTenantContext<AppTenantInfo>
            //    {
            //        TenantInfo = tenant
            //    };
            //    accessor.MultiTenantContext = context; // No cast to generic interface needed
            //}
            //else
            //{
            //    context.TenantInfo = tenant;
            //}


            using var scope = _serviceProvider.CreateScope();

            // Get all registered implementations
            var services = scope.ServiceProvider.GetServices<IMultiTenantContextAccessor>();
            var accessor = services.FirstOrDefault();

            // Or try to get a writable version through reflection
            var accessorType = accessor.GetType();
            var contextProperty = accessorType.GetProperty("MultiTenantContext",
                BindingFlags.Public | BindingFlags.Instance);

            if (contextProperty != null && contextProperty.CanWrite)
            {
                var context = new MultiTenantContext<AppTenantInfo>
                {
                    TenantInfo = tenant
                };
                contextProperty.SetValue(accessor, context);
            }


            if (_dbContext.Database.GetMigrations().Any())
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    _logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", tenant.Id);
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (await _dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", tenant.Id);

                    //  await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
                }
            }
            // await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            //     .InitializeAsync(cancellationToken);
        }

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
}
