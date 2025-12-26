using ContentModels.Domain;
using Entries.Api.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entries.Api.Controllers;

[ApiController]
[Authorize]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/content-models/{modelId:guid}/entries")]
public class EntriesController : ControllerBase
{
    private readonly EntryService _service;

    public EntriesController(EntryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entry>>> GetEntries(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromQuery] Guid? environmentId, [FromQuery] string? locale)
    {
        var entries = await _service.GetAsync(tenantId, branchId, modelId, environmentId, locale);
        return Ok(entries);
    }

    [HttpGet("{entryId:guid}")]
    public async Task<ActionResult<Entry>> GetEntry(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, Guid entryId)
    {
        var entry = await _service.GetOneAsync(tenantId, branchId, modelId, entryId);
        return entry is null ? NotFound() : Ok(entry);
    }

    [HttpPost]
    public async Task<ActionResult<Entry>> Create(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] CreateEntryRequest request)
    {
        var entry = await _service.CreateAsync(
            tenantId,
            branchId,
            modelId,
            request.EnvironmentId,
            request.Locale ?? "en-us",
            request.Data ?? new Dictionary<string, object?>(),
            request.Published,
            request.PublishAt,
            request.TaxonomyIds);

        return CreatedAtAction(nameof(GetEntry), new { tenantId, branchId, modelId, entryId = entry.Id }, entry);
    }

    [HttpPut("{entryId:guid}")]
    public async Task<ActionResult> Update(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, [FromBody] UpdateEntryRequest request)
    {
        var updated = await _service.UpdateAsync(
            tenantId,
            branchId,
            modelId,
            entryId,
            request.Data ?? new Dictionary<string, object?>(),
            request.Published,
            request.EnvironmentId,
            request.Locale,
            request.TaxonomyIds);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{entryId:guid}/publish")]
    public async Task<ActionResult> Publish(Guid tenantId, Guid branchId, Guid modelId, Guid entryId)
    {
        var updated = await _service.PublishAsync(tenantId, branchId, modelId, entryId);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{entryId:guid}/unpublish")]
    public async Task<ActionResult> Unpublish(Guid tenantId, Guid branchId, Guid modelId, Guid entryId)
    {
        var updated = await _service.UnpublishAsync(tenantId, branchId, modelId, entryId);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{entryId:guid}/schedule")]
    public async Task<ActionResult> Schedule(Guid tenantId, Guid branchId, Guid modelId, Guid entryId, [FromBody] ScheduleEntryRequest request)
    {
        if (request.PublishAt is null) return BadRequest("PublishAt is required for scheduling.");
        var updated = await _service.SchedulePublishAsync(tenantId, branchId, modelId, entryId, request.PublishAt.Value, request.UnpublishAt);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{entryId:guid}")]
    public async Task<ActionResult> Delete(Guid tenantId, Guid branchId, Guid modelId, Guid entryId)
    {
        var deleted = await _service.DeleteAsync(tenantId, branchId, modelId, entryId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateEntryRequest(Guid EnvironmentId, Dictionary<string, object?>? Data, bool Published, string? Locale, DateTime? PublishAt, List<Guid>? TaxonomyIds);

public record UpdateEntryRequest(Dictionary<string, object?>? Data, bool? Published, Guid? EnvironmentId, string? Locale, List<Guid>? TaxonomyIds);

public record ScheduleEntryRequest(DateTime? PublishAt, DateTime? UnpublishAt);
