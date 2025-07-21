using Finbuckle.MultiTenant;
using Mapster;
using MultiTenantDemo.Interfaces;
using MultiTenantDemo.Models;

namespace MultiTenantDemo.Implementation
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;
        //  private readonly IConnectionStringSecurer _csSecurer;
        private readonly IDatabaseInitializer _dbInitializer;
        //  private readonly IStringLocalizer _t;


        public TenantService(
            IMultiTenantStore<AppTenantInfo> tenantStore,
            //  IConnectionStringSecurer csSecurer,
            IDatabaseInitializer dbInitializer


            )
        {
            _tenantStore = tenantStore;
            //  _csSecurer = csSecurer;
            _dbInitializer = dbInitializer;

            // _dbSettings = dbSettings.Value;
        }

        public async Task<List<TenantDto>> GetAllAsync()
        {
            var tenants = (await _tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();
            //tenants.ForEach(t => t.ConnectionString = _csSecurer.MakeSecure(t.ConnectionString));
            return tenants;
        }

        public async Task<bool> ExistsWithIdAsync(string id) =>
            await _tenantStore.TryGetAsync(id) is not null;

        public async Task<bool> ExistsWithNameAsync(string name) =>
            (await _tenantStore.GetAllAsync()).Any(t => t.Name == name);

        public async Task<TenantDto> GetByIdAsync(string id) =>
            (await GetTenantInfoAsync(id))
                .Adapt<TenantDto>();

        //public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
        //{
        //    if (request.ConnectionString?.Trim() == _dbSettings.ConnectionString.Trim()) request.ConnectionString = string.Empty;

        //    var tenant = new FSHTenantInfo(request.Id, request.Name, request.ConnectionString, request.AdminEmail, request.Issuer);
        //    await _tenantStore.TryAddAsync(tenant);

        //    // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        //    try
        //    {
        //        await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        //    }
        //    catch
        //    {
        //        await _tenantStore.TryRemoveAsync(request.Id);
        //        throw;
        //    }

        //    return tenant.Id;
        //}

        public async Task<string> ActivateAsync(string id)
        {
            var tenant = await GetTenantInfoAsync(id);

            if (tenant.IsActive)
            {
                // throw new ConflictException(_t["Tenant is already Activated."]);
            }

            tenant.Activate();

            await _tenantStore.TryUpdateAsync(tenant);

            return "Tenant {0} is now Activated.";
        }

        public async Task<string> DeactivateAsync(string id)
        {
            var tenant = await GetTenantInfoAsync(id);
            if (!tenant.IsActive)
            {
                //  throw new ConflictException(_t["Tenant is already Deactivated."]);
            }

            tenant.Deactivate();
            await _tenantStore.TryUpdateAsync(tenant);
            return "Tenant {0} is now Deactivated.";
        }

        public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate)
        {
            var tenant = await GetTenantInfoAsync(id);
            tenant.SetValidity(extendedExpiryDate);
            await _tenantStore.TryUpdateAsync(tenant);
            return "Tenant {0}'s Subscription Upgraded. Now Valid till {1}.";
        }

        private async Task<AppTenantInfo> GetTenantInfoAsync(string id) =>
            await _tenantStore.TryGetAsync(id)
                ?? throw new ArgumentException("{0} {1} Not Found.");
    }
}
