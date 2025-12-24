namespace ContentModels.Domain.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetAsync(Guid tenantId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid tenantId, Guid organizationId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
