using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.Data;

public class WebhooksDbContext : DbContext
{
    public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options) : base(options)
    {
    }

    public DbSet<WebhookSubscription> Webhooks => Set<WebhookSubscription>();
}
