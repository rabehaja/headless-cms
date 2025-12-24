using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Roles.Api.Application;

namespace Roles.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/roles")]
public class RolesController : ControllerBase
{
    private readonly RoleService _service;

    public RolesController(RoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles(Guid tenantId)
    {
        var roles = await _service.GetAsync(tenantId);
        return Ok(roles);
    }

    [HttpGet("{roleId:guid}")]
    public async Task<ActionResult<Role>> GetRole(Guid tenantId, Guid roleId)
    {
        var role = await _service.GetOneAsync(tenantId, roleId);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole(Guid tenantId, [FromBody] CreateRoleRequest request)
    {
        var role = await _service.CreateAsync(tenantId, request.Name, request.Permissions ?? new List<string>());
        return CreatedAtAction(nameof(GetRole), new { tenantId, roleId = role.Id }, role);
    }

    [HttpPut("{roleId:guid}")]
    public async Task<ActionResult> UpdateRole(Guid tenantId, Guid roleId, [FromBody] UpdateRoleRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, roleId, request.Name, request.Permissions);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult> DeleteRole(Guid tenantId, Guid roleId)
    {
        var deleted = await _service.DeleteAsync(tenantId, roleId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateRoleRequest(string Name, List<string>? Permissions);

public record UpdateRoleRequest(string? Name, List<string>? Permissions);
