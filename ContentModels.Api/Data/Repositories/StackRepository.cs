using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContentModels.Api.Data.Repositories;

public class StackRepository : IStackRepository
{
    private readonly ContentModelsDbContext _db;

    public StackRepository(ContentModelsDbContext db)
    {
        _db = db;
    }

    public Task<Stack?> GetAsync(Guid stackId, CancellationToken cancellationToken = default) =>
        _db.Stacks.FirstOrDefaultAsync(s => s.Id == stackId, cancellationToken);

    public Task<List<Stack>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _db.Stacks.ToListAsync(cancellationToken);

    public Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _db.Stacks.Where(s => s.OrganizationId == organizationId).ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _db.Stacks.AnyAsync(s => s.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task AddAsync(Stack stack, CancellationToken cancellationToken = default)
    {
        await _db.Stacks.AddAsync(stack, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
