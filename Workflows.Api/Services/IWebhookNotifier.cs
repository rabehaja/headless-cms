namespace Workflows.Api.Services;

public interface IWebhookNotifier
{
    Task NotifyAsync(Guid stackId, Guid tenantId, Guid branchId, string @event, object payload, CancellationToken cancellationToken = default);
}
