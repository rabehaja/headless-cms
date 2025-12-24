using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Webhooks.Api.Services;

public class WebhookDispatcher : BackgroundService
{
    private readonly WebhookQueue _queue;
    private readonly IWebhookRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookDispatcher> _logger;

    public WebhookDispatcher(WebhookQueue queue, IWebhookRepository repository, IHttpClientFactory httpClientFactory, ILogger<WebhookDispatcher> logger)
    {
        _queue = queue;
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _queue.ReadAllAsync(stoppingToken))
        {
            var subs = await _repository.GetByTenantAsync(item.TenantId, stoppingToken);
            var targets = subs.Where(s => s.Active && s.Events.Contains(item.Event, StringComparer.OrdinalIgnoreCase)).ToList();

            foreach (var sub in targets)
            {
                await DeliverAsync(sub, item, stoppingToken);
            }
        }
    }

    private async Task DeliverAsync(WebhookSubscription sub, WebhookEvent evt, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("webhooks");
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(evt.Payload);
        var request = new HttpRequestMessage(HttpMethod.Post, sub.Url)
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };

        var signature = ComputeSignature(sub.Secret, payloadJson);
        request.Headers.Add("X-Webhook-Event", evt.Event);
        request.Headers.Add("X-Webhook-Signature", signature);

        var attempt = 0;
        var success = false;
        while (attempt < sub.MaxRetries && !success)
        {
            attempt++;
            try
            {
                var response = await client.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    success = true;
                }
                else
                {
                    _logger.LogWarning("Webhook delivery failed ({Status}) to {Url} for event {Event}", response.StatusCode, sub.Url, evt.Event);
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Webhook delivery error to {Url} for event {Event}", sub.Url, evt.Event);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
        }
    }

    private static string ComputeSignature(string secret, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret ?? string.Empty));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }
}
