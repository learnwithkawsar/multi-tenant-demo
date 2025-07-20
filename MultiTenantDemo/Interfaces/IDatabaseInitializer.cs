using MultiTenantDemo.Models;

namespace MultiTenantDemo.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task InitializeDatabasesAsync(CancellationToken cancellationToken = default);
        Task InitializeApplicationDbForTenantAsync(FSHTenantInfo tenant, CancellationToken cancellationToken = default);
    }
}
