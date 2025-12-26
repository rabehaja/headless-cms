using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Entries.Api.Application;

public class EntryService
{
    private readonly IEntryRepository _repository;

    public EntryService(IEntryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Entry>> GetAsync(Guid tenantId, Guid branchId, Guid modelId, Guid? environmentId, string? locale, CancellationToken cancellationToken = default) =>
        _repository.GetByModelAsync(tenantId, branchId, modelId, environmentId, locale, cancellationToken);

    public Task<Entry?> GetOneAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default) =>
        _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);

    public async Task<Entry> CreateAsync(Guid tenantId, Guid branchId, Guid modelId, Guid environmentId, string locale, Dictionary<string, object?> data, bool published, DateTime? publishAt, List<Guid>? taxonomyIds, CancellationToken cancellationToken = default)
    {
        var entry = new Entry
        {
            TenantId = tenantId,
            BranchId = branchId,
            ContentModelId = modelId,
            EnvironmentId = environmentId,
            Locale = string.IsNullOrWhiteSpace(locale) ? "en-us" : locale,
            Data = data,
            Published = published,
            State = published ? EntryState.Published : EntryState.Draft,
            ScheduledPublishAt = publishAt,
            PublishedAt = published ? DateTime.UtcNow : null,
            TaxonomyIds = taxonomyIds ?? new List<Guid>(),
            Version = 1
        };

        await _repository.AddAsync(entry, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, Dictionary<string, object?> data, bool? published, Guid? environmentId, string? locale, List<Guid>? taxonomyIds, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);
        if (entry is null) return false;

        entry.Data = data;
        if (environmentId is not null) entry.EnvironmentId = environmentId.Value;
        if (!string.IsNullOrWhiteSpace(locale)) entry.Locale = locale.Trim().ToLowerInvariant();
        if (taxonomyIds is not null) entry.TaxonomyIds = taxonomyIds;
        if (published is not null)
        {
            entry.Published = published.Value;
            entry.State = published.Value ? EntryState.Published : EntryState.Unpublished;
            entry.PublishedAt = published.Value ? DateTime.UtcNow : null;
        }
        entry.Version += 1;
        entry.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SchedulePublishAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, DateTime publishAt, DateTime? unpublishAt, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);
        if (entry is null) return false;

        entry.ScheduledPublishAt = publishAt;
        entry.ScheduledUnpublishAt = unpublishAt;
        entry.State = EntryState.Scheduled;
        entry.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> PublishAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);
        if (entry is null) return false;

        entry.State = EntryState.Published;
        entry.Published = true;
        entry.PublishedAt = DateTime.UtcNow;
        entry.ScheduledPublishAt = null;
        entry.ScheduledUnpublishAt = null;
        entry.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UnpublishAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);
        if (entry is null) return false;

        entry.State = EntryState.Unpublished;
        entry.Published = false;
        entry.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.GetAsync(tenantId, branchId, modelId, entryId, cancellationToken);
        if (entry is null) return false;
        await _repository.RemoveAsync(entry, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
