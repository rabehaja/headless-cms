using ContentModels.Domain;
using Environments.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Environments.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/environments")]
public class EnvironmentsController : ControllerBase
{
    private readonly EnvironmentService _service;

    public EnvironmentsController(EnvironmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentDefinition>>> GetEnvironments(Guid tenantId)
    {
        var envs = await _service.GetAsync(tenantId);
        return Ok(envs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EnvironmentDefinition>> GetEnvironment(Guid tenantId, Guid id)
    {
        var env = await _service.GetOneAsync(tenantId, id);
        return env is null ? NotFound() : Ok(env);
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentDefinition>> Create(Guid tenantId, [FromBody] CreateEnvironmentRequest request)
    {
        var env = await _service.CreateAsync(tenantId, request.Name, request.Type ?? "production", request.Description, request.IsDefault);
        return CreatedAtAction(nameof(GetEnvironment), new { tenantId, id = env.Id }, env);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid tenantId, Guid id, [FromBody] UpdateEnvironmentRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Type, request.Description, request.IsDefault);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateEnvironmentRequest(string Name, string? Type, string? Description, bool IsDefault);

public record UpdateEnvironmentRequest(string? Name, string? Type, string? Description, bool? IsDefault);
