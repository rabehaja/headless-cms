namespace ContentModels.Domain.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetAsync(Guid tenantId, Guid stackId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetByStackAsync(Guid stackId, CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid tenantId, Guid stackId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
