namespace ContentModels.Domain.Repositories;

public interface ITaxonomyRepository
{
    Task<TaxonomyGroup?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<List<TaxonomyGroup>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(TaxonomyGroup group, CancellationToken cancellationToken = default);
    Task RemoveAsync(TaxonomyGroup group, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
