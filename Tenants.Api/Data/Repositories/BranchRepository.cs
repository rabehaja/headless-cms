using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly TenantsDbContext _db;

    public BranchRepository(TenantsDbContext db)
    {
        _db = db;
    }

    public Task<Branch?> GetAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _db.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.TenantId == tenantId, cancellationToken);

    public Task<List<Branch>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.Branches.Where(b => b.TenantId == tenantId).ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default) =>
        _db.Branches.AnyAsync(b => b.TenantId == tenantId && b.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task AddAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await _db.Branches.AddAsync(branch, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
