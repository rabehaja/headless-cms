using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Search.Api.Application;

namespace Search.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/search")]
public class SearchController : ControllerBase
{
    private readonly SearchService _service;

    public SearchController(SearchService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchIndexItem>>> Search(Guid tenantId, [FromQuery] string q, [FromQuery] string? locale, [FromQuery] Guid? contentModelId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query is required.");
        var results = await _service.SearchAsync(tenantId, q, contentModelId, locale, page, pageSize);
        return Ok(results);
    }

    [HttpPost("index")]
    public async Task<ActionResult<SearchIndexItem>> Index(Guid tenantId, [FromBody] IndexEntryRequest request)
    {
        var item = await _service.IndexAsync(tenantId, request.EntryId, request.ContentModelId, request.Locale ?? "en-us", request.Text, request.Taxonomies);
        return CreatedAtAction(nameof(Search), new { tenantId, q = request.Text }, item);
    }

    [HttpDelete("{entryId:guid}")]
    public async Task<ActionResult> Delete(Guid tenantId, Guid entryId)
    {
        var deleted = await _service.DeleteAsync(tenantId, entryId);
        return deleted ? NoContent() : NotFound();
    }
}

public record IndexEntryRequest(Guid EntryId, Guid ContentModelId, string Text, string? Locale, List<string>? Taxonomies);
