namespace ContentModels.Domain.Repositories;

public interface ISearchIndexRepository
{
    Task<SearchIndexItem?> GetAsync(Guid tenantId, Guid entryId, CancellationToken cancellationToken = default);
    Task<List<SearchIndexItem>> SearchAsync(Guid tenantId, string[] terms, Guid? contentModelId, string? locale, int skip, int take, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(SearchIndexItem item, CancellationToken cancellationToken = default);
    Task RemoveAsync(SearchIndexItem item, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
