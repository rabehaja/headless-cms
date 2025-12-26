namespace ContentModels.Domain.Repositories;

public interface IContentModelRepository
{
    Task<ContentModel?> GetAsync(Guid tenantId, Guid branchId, Guid modelId, CancellationToken cancellationToken = default);
    Task<List<ContentModel>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid tenantId, Guid branchId, string name, CancellationToken cancellationToken = default);
    Task AddAsync(ContentModel model, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(ContentModel model, CancellationToken cancellationToken = default);
}
