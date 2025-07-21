using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.Constants;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Implementation;
using MultiTenantDemo.Interfaces;
using MultiTenantDemo.Models;

namespace MultiTenantDemo
{
    public static class Extensions
    {
        internal static IServiceCollection AddMultitenancy(this IServiceCollection services)
        {
            return services
                .AddDbContext<TenantDbContext>((p, m) =>
                {
                    // TODO: We should probably add specific dbprovider/connectionstring setting for the tenantDb with a fallback to the main databasesettings
                    string connectionString = "Host=localhost;Database=multitenant_default;Username=postgres;Password=1234";
                    m.UseNpgsql(connectionString);
                })
                .AddMultiTenant<AppTenantInfo>()
                .WithRouteStrategy(MultitenancyConstants.TenantIdName)
                    .WithHeaderStrategy(MultitenancyConstants.TenantIdName)
                    .WithEFCoreStore<TenantDbContext, AppTenantInfo>()
                    .Services
                .AddScoped<ITenantService, TenantService>();
        }
    }
}
