using ContentModels.Domain;
using Environments.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Environments.Api.Controllers;

[ApiController]
[Route("stacks/{stackId:guid}/tenants/{tenantId:guid}/branches/{branchId:guid}/locales")]
public class LocalesController : ControllerBase
{
    private readonly LocaleService _service;

    public LocalesController(LocaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Locale>>> GetLocales(Guid stackId, Guid tenantId, Guid branchId)
    {
        var locales = await _service.GetAsync(tenantId, branchId);
        return Ok(locales);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Locale>> GetLocale(Guid stackId, Guid tenantId, Guid branchId, Guid id)
    {
        var locale = await _service.GetOneAsync(tenantId, branchId, id);
        return locale is null ? NotFound() : Ok(locale);
    }

    [HttpPost]
    public async Task<ActionResult<Locale>> CreateLocale(Guid stackId, Guid tenantId, Guid branchId, [FromBody] CreateLocaleRequest request)
    {
        var locale = await _service.CreateAsync(tenantId, branchId, request.Code, request.Name, request.Fallback, request.Default);
        return CreatedAtAction(nameof(GetLocale), new { stackId, tenantId, branchId, id = locale.Id }, locale);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateLocale(Guid stackId, Guid tenantId, Guid branchId, Guid id, [FromBody] UpdateLocaleRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, branchId, id, request.Name, request.Fallback, request.Default);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteLocale(Guid stackId, Guid tenantId, Guid branchId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, branchId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateLocaleRequest(string Code, string Name, string? Fallback, bool Default);

public record UpdateLocaleRequest(string? Name, string? Fallback, bool? Default);
