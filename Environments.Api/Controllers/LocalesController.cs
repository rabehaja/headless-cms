using ContentModels.Domain;
using Environments.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Environments.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/locales")]
public class LocalesController : ControllerBase
{
    private readonly LocaleService _service;

    public LocalesController(LocaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Locale>>> GetLocales(Guid tenantId)
    {
        var locales = await _service.GetAsync(tenantId);
        return Ok(locales);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Locale>> GetLocale(Guid tenantId, Guid id)
    {
        var locale = await _service.GetOneAsync(tenantId, id);
        return locale is null ? NotFound() : Ok(locale);
    }

    [HttpPost]
    public async Task<ActionResult<Locale>> CreateLocale(Guid tenantId, [FromBody] CreateLocaleRequest request)
    {
        var locale = await _service.CreateAsync(tenantId, request.Code, request.Name, request.Fallback, request.Default);
        return CreatedAtAction(nameof(GetLocale), new { tenantId, id = locale.Id }, locale);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateLocale(Guid tenantId, Guid id, [FromBody] UpdateLocaleRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, id, request.Name, request.Fallback, request.Default);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteLocale(Guid tenantId, Guid id)
    {
        var deleted = await _service.DeleteAsync(tenantId, id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateLocaleRequest(string Code, string Name, string? Fallback, bool Default);

public record UpdateLocaleRequest(string? Name, string? Fallback, bool? Default);
