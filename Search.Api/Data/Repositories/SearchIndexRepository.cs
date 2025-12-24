using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Search.Api.Data.Repositories;

public class SearchIndexRepository : ISearchIndexRepository
{
    private readonly SearchDbContext _db;

    public SearchIndexRepository(SearchDbContext db)
    {
        _db = db;
    }

    public Task<SearchIndexItem?> GetAsync(Guid tenantId, Guid entryId, CancellationToken cancellationToken = default) =>
        _db.Index.FirstOrDefaultAsync(i => i.TenantId == tenantId && i.EntryId == entryId, cancellationToken);

    public Task<List<SearchIndexItem>> SearchAsync(Guid tenantId, string[] terms, Guid? contentModelId, string? locale, int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = _db.Index.AsQueryable()
            .Where(i => i.TenantId == tenantId)
            .Where(i => string.IsNullOrWhiteSpace(locale) || i.Locale == locale)
            .Where(i => contentModelId == null || i.ContentModelId == contentModelId);

        foreach (var term in terms)
        {
            var lower = term.ToLower();
            query = query.Where(i => i.Text.ToLower().Contains(lower));
        }

        return query
            .OrderByDescending(i => i.IndexedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddOrUpdateAsync(SearchIndexItem item, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(item.TenantId, item.EntryId, cancellationToken);
        if (existing is null)
        {
            await _db.Index.AddAsync(item, cancellationToken);
        }
        else
        {
            existing.Text = item.Text;
            existing.Locale = item.Locale;
            existing.IndexedAt = DateTime.UtcNow;
        }
    }

    public Task RemoveAsync(SearchIndexItem item, CancellationToken cancellationToken = default)
    {
        _db.Index.Remove(item);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
