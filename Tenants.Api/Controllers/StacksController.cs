using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Application;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("organizations/{organizationId:guid}/stacks")]
public class StacksController : ControllerBase
{
    private readonly StackService _service;

    public StacksController(StackService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Stack>>> GetStacks(Guid organizationId)
    {
        var stacks = await _service.GetByOrganizationAsync(organizationId);
        return Ok(stacks);
    }

    [HttpGet("{stackId:guid}")]
    public async Task<ActionResult<Stack>> GetStack(Guid organizationId, Guid stackId)
    {
        var stack = await _service.GetAsync(organizationId, stackId);
        return stack is null ? NotFound() : Ok(stack);
    }

    [HttpPost]
    public async Task<ActionResult<Stack>> CreateStack(Guid organizationId, [FromBody] CreateStackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Stack name is required.");
        }

        try
        {
            var stack = await _service.CreateAsync(organizationId, request.Name);
            return CreatedAtAction(nameof(GetStack), new { organizationId, stackId = stack.Id }, stack);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record CreateStackRequest(string Name);
