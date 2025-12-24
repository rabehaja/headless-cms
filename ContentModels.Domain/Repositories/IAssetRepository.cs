namespace ContentModels.Domain.Repositories;

public interface IAssetRepository
{
    Task<Asset?> GetAsync(Guid tenantId, Guid assetId, CancellationToken cancellationToken = default);
    Task<List<Asset>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
