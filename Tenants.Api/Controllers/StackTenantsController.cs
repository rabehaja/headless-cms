using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Application;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("organizations/{organizationId:guid}/stacks/{stackId:guid}/tenants")]
public class StackTenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public StackTenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants(Guid organizationId, Guid stackId)
    {
        var tenants = await _tenantService.GetByStackAsync(organizationId, stackId);
        return Ok(tenants);
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> CreateTenant(Guid organizationId, Guid stackId, [FromBody] CreateTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Tenant name is required.");
        }

        try
        {
            var tenant = await _tenantService.CreateAsync(organizationId, request.Name, stackId);
            return CreatedAtAction(nameof(TenantsController.GetTenant), "Tenants", new { organizationId, tenantId = tenant.Id }, tenant);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
