namespace ContentModels.Domain.Repositories;

public interface IWebhookRepository
{
    Task<WebhookSubscription?> GetAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default);
    Task<List<WebhookSubscription>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default);
    Task RemoveAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
