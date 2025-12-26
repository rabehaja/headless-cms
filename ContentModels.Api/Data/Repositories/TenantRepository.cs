using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContentModels.Api.Data.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ContentModelsDbContext _db;

    public TenantRepository(ContentModelsDbContext db)
    {
        _db = db;
    }

    public Task<Tenant?> GetAsync(Guid tenantId, Guid stackId, CancellationToken cancellationToken = default) =>
        _db.Tenants
            .Include(t => t.Branches)
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.StackId == stackId, cancellationToken);

    public Task<List<Tenant>> GetByStackAsync(Guid stackId, CancellationToken cancellationToken = default) =>
        _db.Tenants
            .Include(t => t.Branches)
            .Where(t => t.StackId == stackId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _db.Tenants.AddAsync(tenant, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid tenantId, Guid stackId, CancellationToken cancellationToken = default) =>
        _db.Tenants.AnyAsync(t => t.Id == tenantId && t.StackId == stackId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
