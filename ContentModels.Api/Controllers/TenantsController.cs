using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("organizations/{organizationId:guid}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants(Guid organizationId)
    {
        var tenants = await _tenantService.GetByOrganizationAsync(organizationId);
        return Ok(tenants);
    }

    [HttpGet("{tenantId:guid}")]
    public async Task<ActionResult<Tenant>> GetTenant(Guid organizationId, Guid tenantId)
    {
        var tenant = await _tenantService.GetAsync(organizationId, tenantId);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> CreateTenant(Guid organizationId, [FromBody] CreateTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Tenant name is required.");
        }

        try
        {
            var tenant = await _tenantService.CreateAsync(organizationId, request.Name);
            return CreatedAtAction(nameof(GetTenant), new { organizationId, tenantId = tenant.Id }, tenant);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Organization {organizationId} not found.");
        }
    }
}

public record CreateTenantRequest(string Name);
