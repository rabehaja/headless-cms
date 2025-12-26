using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Application;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches")]
public class BranchesController : ControllerBase
{
    private readonly BranchService _branchService;

    public BranchesController(BranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Branch>>> GetBranches(Guid stackId, Guid tenantId)
    {
        try
        {
            var branches = await _branchService.GetByTenantAsync(stackId, tenantId);
            return Ok(branches);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Tenant {tenantId} not found.");
        }
    }

    [HttpGet("{branchId:guid}")]
    public async Task<ActionResult<Branch>> GetBranch(Guid stackId, Guid tenantId, Guid branchId)
    {
        var branch = await _branchService.GetAsync(stackId, tenantId, branchId);
        return branch is null ? NotFound() : Ok(branch);
    }

    [HttpPost]
    public async Task<ActionResult<Branch>> CreateBranch(Guid stackId, Guid tenantId, [FromBody] CreateBranchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Branch name is required.");
        }

        try
        {
            var branch = await _branchService.CreateAsync(stackId, tenantId, request.Name, request.IsDefault, request.ParentBranchId);
            return CreatedAtAction(nameof(GetBranch), new { stackId, tenantId, branchId = branch.Id }, branch);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record CreateBranchRequest(string Name, bool IsDefault = false, Guid? ParentBranchId = null);
