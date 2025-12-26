using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContentModels.Api.Data.Repositories;

public class ContentModelRepository : IContentModelRepository
{
    private readonly ContentModelsDbContext _db;

    public ContentModelRepository(ContentModelsDbContext db)
    {
        _db = db;
    }

    public Task<ContentModel?> GetAsync(Guid tenantId, Guid branchId, Guid modelId, CancellationToken cancellationToken = default) =>
        _db.ContentModels
            .Include(c => c.Fields)
            .FirstOrDefaultAsync(c => c.Id == modelId && c.TenantId == tenantId && c.BranchId == branchId, cancellationToken);

    public Task<List<ContentModel>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _db.ContentModels
            .Include(c => c.Fields)
            .Where(c => c.TenantId == tenantId && c.BranchId == branchId)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(Guid tenantId, Guid branchId, string name, CancellationToken cancellationToken = default) =>
        _db.ContentModels.AnyAsync(c => c.TenantId == tenantId && c.BranchId == branchId && c.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task AddAsync(ContentModel model, CancellationToken cancellationToken = default)
    {
        await _db.ContentModels.AddAsync(model, cancellationToken);
    }

    public async Task RemoveAsync(ContentModel model, CancellationToken cancellationToken = default)
    {
        _db.ContentModels.Remove(model);
        await Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
