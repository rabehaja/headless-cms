using System.Net.Http.Json;

namespace Workflows.Api.Services;

public class WebhookNotifier : IWebhookNotifier
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookNotifier> _logger;
    private readonly string _baseUrl;

    public WebhookNotifier(HttpClient httpClient, IConfiguration configuration, ILogger<WebhookNotifier> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["Webhooks:BaseUrl"] ?? "http://webhooks-api:8080";
    }

    public async Task NotifyAsync(Guid stackId, Guid tenantId, Guid branchId, string @event, object payload, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_baseUrl}/stacks/{stackId}/tenants/{tenantId}/branches/{branchId}/webhooks/dispatch";
            var response = await _httpClient.PostAsJsonAsync(url, new { Event = @event, Payload = payload }, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to notify webhook service: {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error notifying webhook service");
        }
    }
}
