namespace ContentModels.Domain.Repositories;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<List<ApiKey>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
    Task RemoveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
