using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Webhooks.Api.Application;

public class WebhookService
{
    private readonly IWebhookRepository _repo;

    public WebhookService(IWebhookRepository repo)
    {
        _repo = repo;
    }

    public Task<List<WebhookSubscription>> GetAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _repo.GetByBranchAsync(tenantId, branchId, cancellationToken);

    public Task<WebhookSubscription?> GetOneAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, branchId, id, cancellationToken);

    public async Task<WebhookSubscription> CreateAsync(Guid tenantId, Guid branchId, string name, string url, List<string> events, bool active, string secret, int maxRetries, CancellationToken cancellationToken = default)
    {
        var webhook = new WebhookSubscription
        {
            TenantId = tenantId,
            BranchId = branchId,
            Name = name.Trim(),
            Url = url.Trim(),
            Events = events,
            Active = active,
            Secret = secret,
            MaxRetries = Math.Max(1, maxRetries)
        };

        await _repo.AddAsync(webhook, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return webhook;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid branchId, Guid id, string? name, string? url, List<string>? events, bool? active, string? secret, int? maxRetries, CancellationToken cancellationToken = default)
    {
        var webhook = await _repo.GetAsync(tenantId, branchId, id, cancellationToken);
        if (webhook is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) webhook.Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(url)) webhook.Url = url.Trim();
        if (events is not null) webhook.Events = events;
        if (active is not null) webhook.Active = active.Value;
        if (!string.IsNullOrWhiteSpace(secret)) webhook.Secret = secret.Trim();
        if (maxRetries is not null) webhook.MaxRetries = Math.Max(1, maxRetries.Value);

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _repo.GetAsync(tenantId, branchId, id, cancellationToken);
        if (webhook is null) return false;

        await _repo.RemoveAsync(webhook, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
