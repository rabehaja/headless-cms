using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Delivery.Api.Application;

public class DeliveryService
{
    private readonly IDeliveryEntryRepository _repo;

    public DeliveryService(IDeliveryEntryRepository repo)
    {
        _repo = repo;
    }

    public Task<List<DeliveryEntry>> GetAsync(Guid tenantId, Guid modelId, Guid? environmentId, string? locale, bool includeUnpublished, CancellationToken cancellationToken = default) =>
        _repo.GetByModelAsync(tenantId, modelId, environmentId, locale, includeUnpublished, cancellationToken);

    public Task<DeliveryEntry?> GetOneAsync(Guid tenantId, Guid modelId, Guid entryId, bool includeUnpublished, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, modelId, entryId, includeUnpublished, cancellationToken);

    public async Task<DeliveryEntry> CreateAsync(Guid tenantId, Guid modelId, Guid environmentId, string locale, bool published, Dictionary<string, object?> data, List<Guid>? taxonomyIds, CancellationToken cancellationToken = default)
    {
        var entry = new DeliveryEntry
        {
            TenantId = tenantId,
            ContentModelId = modelId,
            EnvironmentId = environmentId,
            Locale = string.IsNullOrWhiteSpace(locale) ? "en-us" : locale,
            Published = published,
            PublishedAt = published ? DateTime.UtcNow : null,
            Data = data,
            TaxonomyIds = taxonomyIds ?? new List<Guid>()
        };

        await _repo.AddAsync(entry, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return entry;
    }
}
