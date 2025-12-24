namespace ContentModels.Domain.Repositories;

public interface IEnvironmentRepository
{
    Task<EnvironmentDefinition?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<List<EnvironmentDefinition>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(EnvironmentDefinition env, CancellationToken cancellationToken = default);
    Task RemoveAsync(EnvironmentDefinition env, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
