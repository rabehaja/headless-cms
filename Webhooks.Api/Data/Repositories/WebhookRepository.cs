using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.Data.Repositories;

public class WebhookRepository : IWebhookRepository
{
    private readonly WebhooksDbContext _db;

    public WebhookRepository(WebhooksDbContext db)
    {
        _db = db;
    }

    public Task<WebhookSubscription?> GetAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default) =>
        _db.Webhooks.FirstOrDefaultAsync(w => w.Id == id && w.TenantId == tenantId && w.BranchId == branchId, cancellationToken);

    public Task<List<WebhookSubscription>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _db.Webhooks.Where(w => w.TenantId == tenantId && w.BranchId == branchId).ToListAsync(cancellationToken);

    public async Task AddAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default)
    {
        await _db.Webhooks.AddAsync(webhook, cancellationToken);
    }

    public Task RemoveAsync(WebhookSubscription webhook, CancellationToken cancellationToken = default)
    {
        _db.Webhooks.Remove(webhook);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
