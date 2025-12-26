using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Assets.Api.Application;

public class AssetService
{
    private readonly IAssetRepository _repository;

    public AssetService(IAssetRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Asset>> GetAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _repository.GetByBranchAsync(tenantId, branchId, cancellationToken);

    public Task<Asset?> GetOneAsync(Guid tenantId, Guid branchId, Guid assetId, CancellationToken cancellationToken = default) =>
        _repository.GetAsync(tenantId, branchId, assetId, cancellationToken);

    public async Task<Asset> CreateAsync(Guid tenantId, Guid branchId, string fileName, string contentType, long sizeBytes, string url, string? description, CancellationToken cancellationToken = default)
    {
        var asset = new Asset
        {
            TenantId = tenantId,
            BranchId = branchId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            Url = url,
            Description = description
        };

        await _repository.AddAsync(asset, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return asset;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid branchId, Guid assetId, string? fileName, string? contentType, long? sizeBytes, string? url, string? description, CancellationToken cancellationToken = default)
    {
        var asset = await _repository.GetAsync(tenantId, branchId, assetId, cancellationToken);
        if (asset is null) return false;

        if (!string.IsNullOrWhiteSpace(fileName)) asset.FileName = fileName;
        if (!string.IsNullOrWhiteSpace(contentType)) asset.ContentType = contentType;
        if (sizeBytes is not null) asset.SizeBytes = sizeBytes.Value;
        if (!string.IsNullOrWhiteSpace(url)) asset.Url = url;
        if (description is not null) asset.Description = description;

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid branchId, Guid assetId, CancellationToken cancellationToken = default)
    {
        var asset = await _repository.GetAsync(tenantId, branchId, assetId, cancellationToken);
        if (asset is null) return false;

        await _repository.RemoveAsync(asset, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
