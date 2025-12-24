using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GlobalFields.Api.Data.Repositories;

public class GlobalFieldRepository : IGlobalFieldRepository
{
    private readonly GlobalFieldsDbContext _db;

    public GlobalFieldRepository(GlobalFieldsDbContext db)
    {
        _db = db;
    }

    public Task<GlobalFieldDefinition?> GetAsync(Guid tenantId, Guid fieldId, CancellationToken cancellationToken = default) =>
        _db.GlobalFields.FirstOrDefaultAsync(f => f.Id == fieldId && f.TenantId == tenantId, cancellationToken);

    public Task<List<GlobalFieldDefinition>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.GlobalFields.Where(f => f.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(GlobalFieldDefinition field, CancellationToken cancellationToken = default)
    {
        await _db.GlobalFields.AddAsync(field, cancellationToken);
    }

    public async Task RemoveAsync(GlobalFieldDefinition field, CancellationToken cancellationToken = default)
    {
        _db.GlobalFields.Remove(field);
        await Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
