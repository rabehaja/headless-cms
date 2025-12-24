using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entries.Api.Data.Repositories;

public class EntryRepository : IEntryRepository
{
    private readonly EntriesDbContext _db;

    public EntryRepository(EntriesDbContext db)
    {
        _db = db;
    }

    public Task<Entry?> GetAsync(Guid tenantId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default) =>
        _db.Entries.FirstOrDefaultAsync(e => e.Id == entryId && e.TenantId == tenantId && e.ContentModelId == modelId, cancellationToken);

    public Task<List<Entry>> GetByModelAsync(Guid tenantId, Guid modelId, Guid? environmentId, string? locale, CancellationToken cancellationToken = default) =>
        _db.Entries
            .Where(e => e.TenantId == tenantId && e.ContentModelId == modelId)
            .Where(e => environmentId == null || e.EnvironmentId == environmentId)
            .Where(e => string.IsNullOrWhiteSpace(locale) || e.Locale == locale)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Entry entry, CancellationToken cancellationToken = default)
    {
        await _db.Entries.AddAsync(entry, cancellationToken);
    }

    public Task RemoveAsync(Entry entry, CancellationToken cancellationToken = default)
    {
        _db.Entries.Remove(entry);
        return Task.CompletedTask;
    }

    public Task<List<Entry>> GetScheduledAsync(DateTime utcNow, CancellationToken cancellationToken = default) =>
        _db.Entries
            .Where(e => e.State == EntryState.Scheduled && e.ScheduledPublishAt != null && e.ScheduledPublishAt <= utcNow)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
