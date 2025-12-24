using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Search.Api.Application;

namespace Search.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/taxonomies")]
public class TaxonomiesController : ControllerBase
{
    private readonly TaxonomyService _service;

    public TaxonomiesController(TaxonomyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxonomyGroup>>> GetTaxonomies(Guid tenantId)
    {
        var taxonomies = await _service.GetAsync(tenantId);
        return Ok(taxonomies);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaxonomyGroup>> GetTaxonomy(Guid tenantId, Guid id)
    {
        var taxonomy = await _service.GetOneAsync(tenantId, id);
        return taxonomy is null ? NotFound() : Ok(taxonomy);
    }

    [HttpPost]
    public async Task<ActionResult<TaxonomyGroup>> CreateTaxonomy(Guid tenantId, [FromBody] CreateTaxonomyRequest request)
    {
        var taxonomy = await _service.CreateAsync(tenantId, request.Name, request.Terms ?? new List<TaxonomyTerm>());
        return CreatedAtAction(nameof(GetTaxonomy), new { tenantId, id = taxonomy.Id }, taxonomy);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateTaxonomy(Guid tenantId, Guid id, [FromBody] UpdateTaxonomyRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Terms);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTaxonomy(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateTaxonomyRequest(string Name, List<TaxonomyTerm>? Terms);

public record UpdateTaxonomyRequest(string? Name, List<TaxonomyTerm>? Terms);
