using ContentModels.Domain;
using GlobalFields.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFields.Api.Controllers;

[ApiController]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/global-fields")]
public class GlobalFieldsController : ControllerBase
{
    private readonly GlobalFieldService _service;

    public GlobalFieldsController(GlobalFieldService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GlobalFieldDefinition>>> GetFields(Guid stackId, Guid tenantId, Guid branchId)
    {
        var fields = await _service.GetByBranchAsync(tenantId, branchId);
        return Ok(fields);
    }

    [HttpGet("{fieldId:guid}")]
    public async Task<ActionResult<GlobalFieldDefinition>> GetField(Guid stackId, Guid tenantId, Guid branchId, Guid fieldId)
    {
        var field = await _service.GetAsync(tenantId, branchId, fieldId);
        return field is null ? NotFound() : Ok(field);
    }

    [HttpPost]
    public async Task<ActionResult<GlobalFieldDefinition>> CreateField(Guid stackId, Guid tenantId, Guid branchId, [FromBody] CreateGlobalFieldRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Key and name are required.");
        }

        try
        {
            var field = await _service.CreateAsync(tenantId, branchId, request.Key, request.Name, request.Type, request.Required, request.Settings ?? new FieldSettings());
            return CreatedAtAction(nameof(GetField), new { stackId, tenantId, branchId, fieldId = field.Id }, field);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{fieldId:guid}")]
    public async Task<ActionResult> UpdateField(Guid stackId, Guid tenantId, Guid branchId, Guid fieldId, [FromBody] UpdateGlobalFieldRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(tenantId, branchId, fieldId, request.Key, request.Name, request.Type, request.Required, request.Settings);
            return updated ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{fieldId:guid}")]
    public async Task<ActionResult> DeleteField(Guid tenantId, Guid branchId, Guid fieldId)
    {
        var deleted = await _service.DeleteAsync(tenantId, branchId, fieldId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateGlobalFieldRequest(string Key, string Name, FieldType Type, bool Required, FieldSettings? Settings);

public record UpdateGlobalFieldRequest(string? Key, string? Name, FieldType? Type, bool? Required, FieldSettings? Settings);
