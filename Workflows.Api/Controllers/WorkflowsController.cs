using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Workflows.Api.Application;

namespace Workflows.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly WorkflowService _service;

    public WorkflowsController(WorkflowService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workflow>>> GetWorkflows(Guid tenantId)
    {
        var workflows = await _service.GetAsync(tenantId);
        return Ok(workflows);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Workflow>> GetWorkflow(Guid tenantId, Guid id)
    {
        var wf = await _service.GetOneAsync(tenantId, id);
        return wf is null ? NotFound() : Ok(wf);
    }

    [HttpPost]
    public async Task<ActionResult<Workflow>> CreateWorkflow(Guid tenantId, [FromBody] CreateWorkflowRequest request)
    {
        var wf = await _service.CreateAsync(tenantId, request.Name, request.Steps ?? new List<WorkflowStep>());
        return CreatedAtAction(nameof(GetWorkflow), new { tenantId, id = wf.Id }, wf);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateWorkflow(Guid tenantId, Guid id, [FromBody] UpdateWorkflowRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Steps);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteWorkflow(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateWorkflowRequest(string Name, List<WorkflowStep>? Steps);

public record UpdateWorkflowRequest(string? Name, List<WorkflowStep>? Steps);
