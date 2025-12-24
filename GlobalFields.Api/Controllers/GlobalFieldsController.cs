using ContentModels.Domain;
using GlobalFields.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFields.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/global-fields")]
public class GlobalFieldsController : ControllerBase
{
    private readonly GlobalFieldService _service;

    public GlobalFieldsController(GlobalFieldService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GlobalFieldDefinition>>> GetFields(Guid tenantId)
    {
        var fields = await _service.GetByTenantAsync(tenantId);
        return Ok(fields);
    }

    [HttpGet("{fieldId:guid}")]
    public async Task<ActionResult<GlobalFieldDefinition>> GetField(Guid tenantId, Guid fieldId)
    {
        var field = await _service.GetAsync(tenantId, fieldId);
        return field is null ? NotFound() : Ok(field);
    }

    [HttpPost]
    public async Task<ActionResult<GlobalFieldDefinition>> CreateField(Guid tenantId, [FromBody] CreateGlobalFieldRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Key and name are required.");
        }

        try
        {
            var field = await _service.CreateAsync(tenantId, request.Key, request.Name, request.Type, request.Required, request.Settings ?? new FieldSettings());
            return CreatedAtAction(nameof(GetField), new { tenantId, fieldId = field.Id }, field);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{fieldId:guid}")]
    public async Task<ActionResult> UpdateField(Guid tenantId, Guid fieldId, [FromBody] UpdateGlobalFieldRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(tenantId, fieldId, request.Key, request.Name, request.Type, request.Required, request.Settings);
            return updated ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{fieldId:guid}")]
    public async Task<ActionResult> DeleteField(Guid tenantId, Guid fieldId)
    {
        var deleted = await _service.DeleteAsync(tenantId, fieldId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateGlobalFieldRequest(string Key, string Name, FieldType Type, bool Required, FieldSettings? Settings);

public record UpdateGlobalFieldRequest(string? Key, string? Name, FieldType? Type, bool? Required, FieldSettings? Settings);
