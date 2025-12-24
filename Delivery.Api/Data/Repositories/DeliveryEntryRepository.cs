using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Data.Repositories;

public class DeliveryEntryRepository : IDeliveryEntryRepository
{
    private readonly DeliveryDbContext _db;

    public DeliveryEntryRepository(DeliveryDbContext db)
    {
        _db = db;
    }

    public Task<DeliveryEntry?> GetAsync(Guid tenantId, Guid modelId, Guid entryId, bool includeUnpublished, CancellationToken cancellationToken = default) =>
        _db.Entries
            .Where(e => e.TenantId == tenantId && e.ContentModelId == modelId && e.Id == entryId)
            .Where(e => includeUnpublished || e.Published)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<DeliveryEntry>> GetByModelAsync(Guid tenantId, Guid modelId, Guid? environmentId, string? locale, bool includeUnpublished, CancellationToken cancellationToken = default) =>
        _db.Entries
            .Where(e => e.TenantId == tenantId && e.ContentModelId == modelId)
            .Where(e => includeUnpublished || e.Published)
            .Where(e => environmentId == null || e.EnvironmentId == environmentId)
            .Where(e => string.IsNullOrWhiteSpace(locale) || e.Locale == locale)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(DeliveryEntry entry, CancellationToken cancellationToken = default)
    {
        await _db.Entries.AddAsync(entry, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
