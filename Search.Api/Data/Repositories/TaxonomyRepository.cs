using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Search.Api.Data.Repositories;

public class TaxonomyRepository : ITaxonomyRepository
{
    private readonly SearchDbContext _db;

    public TaxonomyRepository(SearchDbContext db)
    {
        _db = db;
    }

    public Task<TaxonomyGroup?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _db.Taxonomies.FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId, cancellationToken);

    public Task<List<TaxonomyGroup>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.Taxonomies.Where(t => t.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(TaxonomyGroup group, CancellationToken cancellationToken = default)
    {
        await _db.Taxonomies.AddAsync(group, cancellationToken);
    }

    public Task RemoveAsync(TaxonomyGroup group, CancellationToken cancellationToken = default)
    {
        _db.Taxonomies.Remove(group);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
