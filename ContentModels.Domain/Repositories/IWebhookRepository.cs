namespace ContentModels.Domain.Repositories;

public interface IWebhookRepository
{
    Task<WebhookSubscription?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<List<WebhookSubscription>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default);
    Task RemoveAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
