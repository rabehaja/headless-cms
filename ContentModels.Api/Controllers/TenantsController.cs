using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("stacks/{stackId:guid}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants(Guid stackId)
    {
        var tenants = await _tenantService.GetByStackAsync(stackId);
        return Ok(tenants);
    }

    [HttpGet("{tenantId:guid}")]
    public async Task<ActionResult<Tenant>> GetTenant(Guid stackId, Guid tenantId)
    {
        var tenant = await _tenantService.GetAsync(stackId, tenantId);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> CreateTenant(Guid stackId, [FromBody] CreateTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Tenant name is required.");
        }

        var tenant = await _tenantService.CreateAsync(stackId, request.Name);
        return CreatedAtAction(nameof(GetTenant), new { stackId, tenantId = tenant.Id }, tenant);
    }
}

public record CreateTenantRequest(string Name);
