namespace ContentModels.Domain.Repositories;

public interface IContentModelRepository
{
    Task<ContentModel?> GetAsync(Guid tenantId, Guid modelId, CancellationToken cancellationToken = default);
    Task<List<ContentModel>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task AddAsync(ContentModel model, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(ContentModel model, CancellationToken cancellationToken = default);
}
