using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly TenantsDbContext _db;

    public TenantRepository(TenantsDbContext db)
    {
        _db = db;
    }

    public Task<Tenant?> GetAsync(Guid tenantId, Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId && t.OrganizationId == organizationId, cancellationToken);

    public Task<List<Tenant>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Tenants.Where(t => t.OrganizationId == organizationId).ToListAsync(cancellationToken);

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _db.Tenants.AddAsync(tenant, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid tenantId, Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Tenants.AnyAsync(t => t.Id == tenantId && t.OrganizationId == organizationId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
