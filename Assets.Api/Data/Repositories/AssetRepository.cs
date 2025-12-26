using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Assets.Api.Data.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AssetsDbContext _db;

    public AssetRepository(AssetsDbContext db)
    {
        _db = db;
    }

    public Task<Asset?> GetAsync(Guid tenantId, Guid branchId, Guid assetId, CancellationToken cancellationToken = default) =>
        _db.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.TenantId == tenantId && a.BranchId == branchId, cancellationToken);

    public Task<List<Asset>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _db.Assets.Where(a => a.TenantId == tenantId && a.BranchId == branchId).ToListAsync(cancellationToken);

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await _db.Assets.AddAsync(asset, cancellationToken);
    }

    public Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _db.Assets.Remove(asset);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
