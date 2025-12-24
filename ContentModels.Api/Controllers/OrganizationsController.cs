using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("organizations")]
public class OrganizationsController : ControllerBase
{
    private readonly OrganizationService _organizationService;

    public OrganizationsController(OrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations() =>
        Ok(await _organizationService.GetAllAsync());

    [HttpGet("{organizationId:guid}")]
    public async Task<ActionResult<Organization>> GetOrganization(Guid organizationId)
    {
        var org = await _organizationService.GetAsync(organizationId);
        return org is null ? NotFound() : Ok(org);
    }

    [HttpPost]
    public async Task<ActionResult<Organization>> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Organization name is required.");
        }

        var org = await _organizationService.CreateAsync(request.Name);
        return CreatedAtAction(nameof(GetOrganization), new { organizationId = org.Id }, org);
    }
}

public record CreateOrganizationRequest(string Name);
