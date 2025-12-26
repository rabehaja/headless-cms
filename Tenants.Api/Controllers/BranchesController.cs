using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Application;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("organizations/{organizationId:guid}/tenants/{tenantId:guid}/branches")]
public class BranchesController : ControllerBase
{
    private readonly BranchService _branchService;

    public BranchesController(BranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Branch>>> GetBranches(Guid organizationId, Guid tenantId)
    {
        try
        {
            var branches = await _branchService.GetByTenantAsync(organizationId, tenantId);
            return Ok(branches);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Tenant {tenantId} not found.");
        }
    }

    [HttpGet("{branchId:guid}")]
    public async Task<ActionResult<Branch>> GetBranch(Guid organizationId, Guid tenantId, Guid branchId)
    {
        var branch = await _branchService.GetAsync(organizationId, tenantId, branchId);
        return branch is null ? NotFound() : Ok(branch);
    }

    [HttpPost]
    public async Task<ActionResult<Branch>> CreateBranch(Guid organizationId, Guid tenantId, [FromBody] CreateBranchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Branch name is required.");
        }

        try
        {
            var branch = await _branchService.CreateAsync(organizationId, tenantId, request.Name);
            return CreatedAtAction(nameof(GetBranch), new { organizationId, tenantId, branchId = branch.Id }, branch);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record CreateBranchRequest(string Name);
