using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Roles.Api.Application;

namespace Roles.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/api-keys")]
public class ApiKeysController : ControllerBase
{
    private readonly ApiKeyService _service;

    public ApiKeysController(ApiKeyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApiKey>>> GetKeys(Guid tenantId)
    {
        var keys = await _service.GetAsync(tenantId);
        return Ok(keys);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiKey>> GetKey(Guid tenantId, Guid id)
    {
        var key = await _service.GetOneAsync(tenantId, id);
        return key is null ? NotFound() : Ok(key);
    }

    [HttpPost]
    public async Task<ActionResult<ApiKey>> CreateKey(Guid tenantId, [FromBody] CreateApiKeyRequest request)
    {
        var key = await _service.CreateAsync(tenantId, request.Name, request.Type ?? "delivery");
        return CreatedAtAction(nameof(GetKey), new { tenantId, id = key.Id }, key);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateKey(Guid tenantId, Guid id, [FromBody] UpdateApiKeyRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Type, request.Active);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteKey(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateApiKeyRequest(string Name, string? Type);

public record UpdateApiKeyRequest(string? Name, string? Type, bool? Active);
