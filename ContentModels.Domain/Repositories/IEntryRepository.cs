namespace ContentModels.Domain.Repositories;

public interface IEntryRepository
{
    Task<Entry?> GetAsync(Guid tenantId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default);
    Task<List<Entry>> GetByModelAsync(Guid tenantId, Guid modelId, Guid? environmentId, string? locale, CancellationToken cancellationToken = default);
    Task AddAsync(Entry entry, CancellationToken cancellationToken = default);
    Task RemoveAsync(Entry entry, CancellationToken cancellationToken = default);
    Task<List<Entry>> GetScheduledAsync(DateTime utcNow, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
