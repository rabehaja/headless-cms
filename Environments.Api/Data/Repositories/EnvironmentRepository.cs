using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Environments.Api.Data.Repositories;

public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly EnvironmentsDbContext _db;

    public EnvironmentRepository(EnvironmentsDbContext db)
    {
        _db = db;
    }

    public Task<EnvironmentDefinition?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _db.Environments.FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId, cancellationToken);

    public Task<List<EnvironmentDefinition>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.Environments.Where(e => e.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(EnvironmentDefinition env, CancellationToken cancellationToken = default)
    {
        await _db.Environments.AddAsync(env, cancellationToken);
    }

    public Task RemoveAsync(EnvironmentDefinition env, CancellationToken cancellationToken = default)
    {
        _db.Environments.Remove(env);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
