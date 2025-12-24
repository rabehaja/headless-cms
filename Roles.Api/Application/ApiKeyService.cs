using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Roles.Api.Application;

public class ApiKeyService
{
    private readonly IApiKeyRepository _repo;

    public ApiKeyService(IApiKeyRepository repo)
    {
        _repo = repo;
    }

    public Task<List<ApiKey>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _repo.GetByTenantAsync(tenantId, cancellationToken);

    public Task<ApiKey?> GetOneAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, id, cancellationToken);

    public async Task<ApiKey> CreateAsync(Guid tenantId, string name, string type, CancellationToken cancellationToken = default)
    {
        var apiKey = new ApiKey
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Type = string.IsNullOrWhiteSpace(type) ? "delivery" : type.Trim(),
            Key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            Active = true
        };

        await _repo.AddAsync(apiKey, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return apiKey;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid id, string? name, string? type, bool? active, CancellationToken cancellationToken = default)
    {
        var apiKey = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (apiKey is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) apiKey.Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(type)) apiKey.Type = type.Trim();
        if (active is not null) apiKey.Active = active.Value;

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var apiKey = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (apiKey is null) return false;

        await _repo.RemoveAsync(apiKey, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
