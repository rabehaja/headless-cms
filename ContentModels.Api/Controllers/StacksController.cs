using ContentModels.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("stacks")]
public class StacksController : ControllerBase
{
    private readonly StackService _service;

    public StacksController(StackService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Stack>>> GetStacks([FromQuery] Guid? organizationId)
    {
        var stacks = organizationId is null
            ? await _service.GetAllAsync()
            : await _service.GetByOrganizationAsync(organizationId.Value);
        return Ok(stacks);
    }

    [HttpGet("{stackId:guid}")]
    public async Task<ActionResult<Stack>> GetStack(Guid stackId)
    {
        var stack = await _service.GetAsync(stackId);
        return stack is null ? NotFound() : Ok(stack);
    }

    [HttpPost]
    public async Task<ActionResult<Stack>> CreateStack([FromBody] CreateStackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Stack name is required.");
        }

        try
        {
            var stack = await _service.CreateAsync(request.Name, request.OrganizationId);
            return CreatedAtAction(nameof(GetStack), new { stackId = stack.Id }, stack);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record CreateStackRequest(string Name, Guid? OrganizationId);
