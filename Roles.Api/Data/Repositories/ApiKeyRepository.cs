using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Roles.Api.Data.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly RolesDbContext _db;

    public ApiKeyRepository(RolesDbContext db)
    {
        _db = db;
    }

    public Task<ApiKey?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _db.ApiKeys.FirstOrDefaultAsync(k => k.Id == id && k.TenantId == tenantId, cancellationToken);

    public Task<List<ApiKey>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.ApiKeys.Where(k => k.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        await _db.ApiKeys.AddAsync(apiKey, cancellationToken);
    }

    public Task RemoveAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
    {
        _db.ApiKeys.Remove(apiKey);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
