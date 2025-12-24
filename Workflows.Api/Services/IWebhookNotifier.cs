namespace Workflows.Api.Services;

public interface IWebhookNotifier
{
    Task NotifyAsync(Guid tenantId, string @event, object payload, CancellationToken cancellationToken = default);
}
