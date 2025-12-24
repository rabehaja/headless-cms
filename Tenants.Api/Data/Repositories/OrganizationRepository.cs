using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly TenantsDbContext _db;

    public OrganizationRepository(TenantsDbContext db)
    {
        _db = db;
    }

    public Task<Organization?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Organizations.Include(o => o.Tenants).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _db.Organizations.Include(o => o.Tenants).ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Organizations.AnyAsync(o => o.Id == id, cancellationToken);

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _db.Organizations.AddAsync(organization, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
