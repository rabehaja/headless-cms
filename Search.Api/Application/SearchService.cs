using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Search.Api.Services;

namespace Search.Api.Application;

public class SearchService
{
    private readonly ISearchIndexRepository _repo;
    private readonly ElasticIndexer _elastic;

    public SearchService(ISearchIndexRepository repo, ElasticIndexer elastic)
    {
        _repo = repo;
        _elastic = elastic;
    }

    public async Task<List<SearchIndexItem>> SearchAsync(Guid tenantId, string query, Guid? contentModelId, string? locale, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var skip = Math.Max(page - 1, 0) * Math.Max(pageSize, 1);
        var take = Math.Min(Math.Max(pageSize, 1), 100);
        var elasticResults = await _elastic.SearchAsync(tenantId, terms, contentModelId, locale, skip, take, cancellationToken);
        // Fallback to DB if elastic is empty
        if (elasticResults.Any()) return elasticResults;
        return await _repo.SearchAsync(tenantId, terms, contentModelId, locale, skip, take, cancellationToken);
    }

    public Task<SearchIndexItem?> GetAsync(Guid tenantId, Guid entryId, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, entryId, cancellationToken);

    public async Task<SearchIndexItem> IndexAsync(Guid tenantId, Guid entryId, Guid modelId, string locale, string text, List<string>? taxonomies, CancellationToken cancellationToken = default)
    {
        var item = new SearchIndexItem
        {
            TenantId = tenantId,
            EntryId = entryId,
            ContentModelId = modelId,
            Locale = string.IsNullOrWhiteSpace(locale) ? "en-us" : locale,
            Text = text,
            IndexedAt = DateTime.UtcNow,
            Taxonomies = taxonomies ?? new List<string>()
        };

        await _repo.AddOrUpdateAsync(item, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _elastic.IndexAsync(item, cancellationToken);
        return item;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid entryId, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetAsync(tenantId, entryId, cancellationToken);
        if (existing is null) return false;
        await _repo.RemoveAsync(existing, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _elastic.DeleteAsync(existing.Id, cancellationToken);
        return true;
    }
}
