using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Entries.Api.Application;

public class EntrySchedulingService
{
    private readonly IEntryRepository _entries;
    private readonly ILogger<EntrySchedulingService> _logger;

    public EntrySchedulingService(IEntryRepository entries, ILogger<EntrySchedulingService> logger)
    {
        _entries = entries;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var due = await _entries.GetScheduledAsync(now, cancellationToken);
        foreach (var entry in due)
        {
            entry.State = EntryState.Published;
            entry.Published = true;
            entry.PublishedAt = now;
            entry.UpdatedAt = now;
            entry.ScheduledPublishAt = null;
        }

        if (due.Count > 0)
        {
            _logger.LogInformation("Published {Count} scheduled entries", due.Count);
            await _entries.SaveChangesAsync(cancellationToken);
        }
    }
}
