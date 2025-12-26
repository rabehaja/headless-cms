using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/content-models")]
public class ContentModelsController : ControllerBase
{
    private readonly ContentModelService _contentModelService;

    public ContentModelsController(ContentModelService contentModelService)
    {
        _contentModelService = contentModelService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContentModel>>> GetModels(Guid stackId, Guid tenantId, Guid branchId)
    {
        try
        {
            var models = await _contentModelService.GetByBranchAsync(stackId, tenantId, branchId);
            return Ok(models);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Tenant {tenantId} not found.");
        }
    }

    [HttpGet("{modelId:guid}")]
    public async Task<ActionResult<ContentModel>> GetModel(Guid stackId, Guid tenantId, Guid branchId, Guid modelId)
    {
        var model = await _contentModelService.GetAsync(stackId, tenantId, branchId, modelId);
        return model is null ? NotFound() : Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<ContentModel>> CreateModel(Guid stackId, Guid tenantId, Guid branchId, [FromBody] CreateContentModelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Model name is required.");
        }

        try
        {
            var model = await _contentModelService.CreateAsync(
                stackId,
                tenantId,
                branchId,
                request.Name,
                request.Description,
                request.Fields ?? new List<FieldDefinition>(),
                request.Settings ?? new ContentModelSettings());

            return CreatedAtAction(nameof(GetModel), new { stackId, tenantId, branchId, modelId = model.Id }, model);
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
    public async Task<ActionResult> UpdateModel(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] UpdateContentModelRequest request)
    {
        try
        {
            var updated = await _contentModelService.UpdateAsync(
                stackId,
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
    public async Task<ActionResult> UpdateSettings(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, [FromBody] UpdateContentModelSettingsRequest request)
    {
        if (request.Settings is null)
        {
            return BadRequest("Settings payload is required.");
        }

        var updated = await _contentModelService.UpdateSettingsAsync(stackId, tenantId, branchId, modelId, request.Settings);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{modelId:guid}")]
    public async Task<ActionResult> DeleteModel(Guid stackId, Guid tenantId, Guid branchId, Guid modelId)
    {
        var deleted = await _contentModelService.DeleteAsync(stackId, tenantId, branchId, modelId);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("~/stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{sourceBranchId:guid}/content-models/merge-preview/{targetBranchId:guid}")]
    public async Task<ActionResult<ContentModelMergePreview>> PreviewMerge(Guid stackId, Guid tenantId, Guid sourceBranchId, Guid targetBranchId)
    {
        try
        {
            var preview = await _contentModelService.PreviewMergeAsync(stackId, tenantId, sourceBranchId, targetBranchId);
            return Ok(preview);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("~/stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{sourceBranchId:guid}/content-models/merge-apply/{targetBranchId:guid}")]
    public async Task<ActionResult> ApplyMerge(Guid stackId, Guid tenantId, Guid sourceBranchId, Guid targetBranchId)
    {
        try
        {
            var applied = await _contentModelService.ApplyMergeAsync(stackId, tenantId, sourceBranchId, targetBranchId);
            return applied ? NoContent() : Conflict("Conflicts detected; resolve and retry.");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record CreateContentModelRequest(string Name, string? Description, List<FieldDefinition>? Fields, ContentModelSettings? Settings);

public record UpdateContentModelRequest(string? Name, string? Description, List<FieldDefinition>? Fields, ContentModelSettings? Settings);

public record UpdateContentModelSettingsRequest(ContentModelSettings? Settings);
