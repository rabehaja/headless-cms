using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Application;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationsController : ControllerBase
{
    private readonly OrganizationService _service;

    public OrganizationsController(OrganizationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{organizationId:guid}")]
    public async Task<ActionResult<Organization>> GetOrganization(Guid organizationId)
    {
        var org = await _service.GetAsync(organizationId);
        return org is null ? NotFound() : Ok(org);
    }

    [HttpPost]
    public async Task<ActionResult<Organization>> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Organization name is required.");
        }

        var org = await _service.CreateAsync(request.Name);
        return CreatedAtAction(nameof(GetOrganization), new { organizationId = org.Id }, org);
    }
}

public record CreateOrganizationRequest(string Name);
