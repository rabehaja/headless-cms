using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data.Repositories;

public class StackRepository : IStackRepository
{
    private readonly TenantsDbContext _db;

    public StackRepository(TenantsDbContext db)
    {
        _db = db;
    }

    public Task<Stack?> GetAsync(Guid stackId, Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Stacks.FirstOrDefaultAsync(s => s.Id == stackId && s.OrganizationId == organizationId, cancellationToken);

    public Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Stacks.Where(s => s.OrganizationId == organizationId).ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(Guid organizationId, string name, CancellationToken cancellationToken = default) =>
        _db.Stacks.AnyAsync(s => s.OrganizationId == organizationId && s.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task AddAsync(Stack stack, CancellationToken cancellationToken = default)
    {
        await _db.Stacks.AddAsync(stack, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
