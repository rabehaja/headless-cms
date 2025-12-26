using Assets.Api.Application;
using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Assets.Api.Controllers;

[ApiController]
[Route("tenants/{tenantId:guid}/branches/{branchId:guid}/assets")]
public class AssetsController : ControllerBase
{
    private readonly AssetService _service;

    public AssetsController(AssetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets(Guid tenantId, Guid branchId)
    {
        var assets = await _service.GetAsync(tenantId, branchId);
        return Ok(assets);
    }

    [HttpGet("{assetId:guid}")]
    public async Task<ActionResult<Asset>> GetAsset(Guid tenantId, Guid branchId, Guid assetId)
    {
        var asset = await _service.GetOneAsync(tenantId, branchId, assetId);
        return asset is null ? NotFound() : Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> CreateAsset(Guid tenantId, Guid branchId, [FromBody] CreateAssetRequest request)
    {
        var asset = await _service.CreateAsync(tenantId, branchId, request.FileName, request.ContentType, request.SizeBytes, request.Url, request.Description);
        return CreatedAtAction(nameof(GetAsset), new { tenantId, branchId, assetId = asset.Id }, asset);
    }

    [HttpPut("{assetId:guid}")]
    public async Task<ActionResult> UpdateAsset(Guid tenantId, Guid branchId, Guid assetId, [FromBody] UpdateAssetRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, branchId, assetId, request.FileName, request.ContentType, request.SizeBytes, request.Url, request.Description);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{assetId:guid}")]
    public async Task<ActionResult> DeleteAsset(Guid tenantId, Guid branchId, Guid assetId)
    {
        var deleted = await _service.DeleteAsync(tenantId, branchId, assetId);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateAssetRequest(string FileName, string ContentType, long SizeBytes, string Url, string? Description);

public record UpdateAssetRequest(string? FileName, string? ContentType, long? SizeBytes, string? Url, string? Description);
