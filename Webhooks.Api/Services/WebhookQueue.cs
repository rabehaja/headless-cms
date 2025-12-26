using System.Threading.Channels;

namespace Webhooks.Api.Services;

public record WebhookEvent(Guid TenantId, Guid BranchId, string Event, object? Payload);

public class WebhookQueue
{
    private readonly Channel<WebhookEvent> _channel = Channel.CreateUnbounded<WebhookEvent>();

    public ValueTask EnqueueAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(webhookEvent, cancellationToken);

    public IAsyncEnumerable<WebhookEvent> ReadAllAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
