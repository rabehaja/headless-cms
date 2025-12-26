using ContentModels.Domain;
using Delivery.Api.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers;

[ApiController]
[Authorize]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/content-models/{modelId:guid}/entries")]
public class EntriesController : ControllerBase
{
    private readonly DeliveryService _service;

    public EntriesController(DeliveryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryEntry>>> GetEntries(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromQuery] Guid? environmentId, [FromQuery] string? locale, [FromQuery] bool preview = false)
    {
        var entries = await _service.GetAsync(tenantId, branchId, modelId, environmentId, locale, preview);
        Response.Headers.CacheControl = "public, max-age=60";
        return Ok(entries);
    }

    [HttpGet("{entryId:guid}")]
    public async Task<ActionResult<DeliveryEntry>> GetEntry(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, Guid entryId, [FromQuery] bool preview = false)
    {
        var entry = await _service.GetOneAsync(tenantId, branchId, modelId, entryId, preview);
        if (entry is null) return NotFound();
        Response.Headers.CacheControl = "public, max-age=120";
        return Ok(entry);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryEntry>> CreateEntry(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] CreateDeliveryEntryRequest request)
    {
        var entry = await _service.CreateAsync(tenantId, branchId, modelId, request.EnvironmentId, request.Locale ?? "en-us", request.Published, request.Data ?? new Dictionary<string, object?>(), request.TaxonomyIds);
        return CreatedAtAction(nameof(GetEntry), new { stackId, tenantId, branchId, modelId, entryId = entry.Id }, entry);
    }
}

public record CreateDeliveryEntryRequest(Guid EnvironmentId, Dictionary<string, object?>? Data, bool Published, string? Locale, List<Guid>? TaxonomyIds);
