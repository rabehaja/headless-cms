namespace ContentModels.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task RemoveAsync(Role role, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
