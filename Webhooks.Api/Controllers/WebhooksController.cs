using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Webhooks.Api.Application;
using Webhooks.Api.Services;

namespace Webhooks.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly WebhookService _service;
    private readonly WebhookQueue _queue;

    public WebhooksController(WebhookService service, WebhookQueue queue)
    {
        _service = service;
        _queue = queue;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebhookSubscription>>> GetWebhooks(Guid tenantId)
    {
        var hooks = await _service.GetAsync(tenantId);
        return Ok(hooks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WebhookSubscription>> GetWebhook(Guid tenantId, Guid id)
    {
        var hook = await _service.GetOneAsync(tenantId, id);
        return hook is null ? NotFound() : Ok(hook);
    }

    [HttpPost]
    public async Task<ActionResult<WebhookSubscription>> CreateWebhook(Guid tenantId, [FromBody] CreateWebhookRequest request)
    {
        var hook = await _service.CreateAsync(tenantId, request.Name, request.Url, request.Events ?? new List<string>(), request.Active, request.Secret ?? Guid.NewGuid().ToString("N"), request.MaxRetries ?? 3);
        return CreatedAtAction(nameof(GetWebhook), new { tenantId, id = hook.Id }, hook);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateWebhook(Guid tenantId, Guid id, [FromBody] UpdateWebhookRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Url, request.Events, request.Active, request.Secret, request.MaxRetries);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteWebhook(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("dispatch")]
    public async Task<ActionResult> Dispatch(Guid tenantId, [FromBody] DispatchWebhookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Event))
        {
            return BadRequest("Event is required.");
        }

        await _queue.EnqueueAsync(new WebhookEvent(tenantId, request.Event, request.Payload ?? new { }));
        return Accepted();
    }
}

public record CreateWebhookRequest(string Name, string Url, List<string>? Events, bool Active, string? Secret, int? MaxRetries);

public record UpdateWebhookRequest(string? Name, string? Url, List<string>? Events, bool? Active, string? Secret, int? MaxRetries);

public record DispatchWebhookRequest(string Event, object? Payload);
