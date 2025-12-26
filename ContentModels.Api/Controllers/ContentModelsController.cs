using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("organizations/{organizationId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/content-models")]
public class ContentModelsController : ControllerBase
{
    private readonly ContentModelService _contentModelService;

    public ContentModelsController(ContentModelService contentModelService)
    {
        _contentModelService = contentModelService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContentModel>>> GetModels(Guid organizationId, Guid tenantId, Guid branchId)
    {
        try
        {
            var models = await _contentModelService.GetByBranchAsync(organizationId, tenantId, branchId);
            return Ok(models);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Tenant {tenantId} not found.");
        }
    }

    [HttpGet("{modelId:guid}")]
    public async Task<ActionResult<ContentModel>> GetModel(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId)
    {
        var model = await _contentModelService.GetAsync(organizationId, tenantId, branchId, modelId);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<ContentModel>> CreateModel(Guid organizationId, Guid tenantId, Guid branchId, [FromBody] CreateContentModelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Model name is required.");
        }

        try
        {
            var model = await _contentModelService.CreateAsync(
                organizationId,
                tenantId,
                branchId,
                request.Name,
                request.Description,
                request.Fields ?? new List<FieldDefinition>(),
                request.Settings ?? new ContentModelSettings());

            return CreatedAtAction(nameof(GetModel), new { organizationId, tenantId, branchId, modelId = model.Id }, model);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{modelId:guid}")]
    public async Task<ActionResult> UpdateModel(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] UpdateContentModelRequest request)
    {
        try
        {
            var updated = await _contentModelService.UpdateAsync(
                organizationId,
                tenantId,
                branchId,
                modelId,
                request.Name,
                request.Description,
                request.Fields,
                request.Settings);

            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{modelId:guid}/settings")]
    public async Task<ActionResult> UpdateSettings(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] UpdateContentModelSettingsRequest request)
    {
        if (request.Settings is null)
        {
            return BadRequest("Settings payload is required.");
        }

        var updated = await _contentModelService.UpdateSettingsAsync(organizationId, tenantId, branchId, modelId, request.Settings);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{modelId:guid}")]
    public async Task<ActionResult> DeleteModel(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId)
    {
        var deleted = await _contentModelService.DeleteAsync(organizationId, tenantId, branchId, modelId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateContentModelRequest(string Name, string? Description, List<FieldDefinition>? Fields, ContentModelSettings? Settings);

public record UpdateContentModelRequest(string? Name, string? Description, List<FieldDefinition>? Fields, ContentModelSettings? Settings);

public record UpdateContentModelSettingsRequest(ContentModelSettings? Settings);
