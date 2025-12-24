namespace ContentModels.Domain.Repositories;

public interface IDeliveryEntryRepository
{
    Task<DeliveryEntry?> GetAsync(Guid tenantId, Guid modelId, Guid entryId, bool includeUnpublished, CancellationToken cancellationToken = default);
    Task<List<DeliveryEntry>> GetByModelAsync(Guid tenantId, Guid modelId, Guid? environmentId, string? locale, bool includeUnpublished, CancellationToken cancellationToken = default);
    Task AddAsync(DeliveryEntry entry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
