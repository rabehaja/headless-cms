using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Workflows.Api.Application;

namespace Workflows.Api.Controllers;

[ApiController]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly WorkflowService _service;

    public WorkflowsController(WorkflowService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workflow>>> GetWorkflows(Guid stackId, Guid tenantId, Guid branchId)
    {
        var workflows = await _service.GetAsync(tenantId, branchId);
        return Ok(workflows);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Workflow>> GetWorkflow(Guid stackId, Guid tenantId, Guid branchId, Guid id)
    {
        var wf = await _service.GetOneAsync(tenantId, branchId, id);
        return wf is null ? NotFound() : Ok(wf);
    }

    [HttpPost]
    public async Task<ActionResult<Workflow>> CreateWorkflow(Guid stackId, Guid tenantId, Guid branchId, [FromBody] CreateWorkflowRequest request)
    {
        var wf = await _service.CreateAsync(stackId, tenantId, branchId, request.Name, request.Steps ?? new List<WorkflowStep>());
        return CreatedAtAction(nameof(GetWorkflow), new { stackId, tenantId, branchId, id = wf.Id }, wf);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateWorkflow(Guid stackId, Guid tenantId, Guid branchId, Guid id, [FromBody] UpdateWorkflowRequest request)
    {
        var updated = await _service.UpdateAsync(stackId, tenantId, branchId, id, request.Name, request.Steps);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteWorkflow(Guid stackId, Guid tenantId, Guid branchId, Guid id)
    {
        var deleted = await _service.DeleteAsync(stackId, tenantId, branchId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateWorkflowRequest(string Name, List<WorkflowStep>? Steps);

public record UpdateWorkflowRequest(string? Name, List<WorkflowStep>? Steps);
