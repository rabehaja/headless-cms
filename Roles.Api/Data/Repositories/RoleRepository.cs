using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Roles.Api.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RolesDbContext _db;

    public RoleRepository(RolesDbContext db)
    {
        _db = db;
    }

    public Task<Role?> GetAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default) =>
        _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId, cancellationToken);

    public Task<List<Role>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.Roles.Where(r => r.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _db.Roles.AddAsync(role, cancellationToken);
    }

    public Task RemoveAsync(Role role, CancellationToken cancellationToken = default)
    {
        _db.Roles.Remove(role);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
