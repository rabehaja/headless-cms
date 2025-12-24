using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Environments.Api.Application;

public class EnvironmentService
{
    private readonly IEnvironmentRepository _repo;

    public EnvironmentService(IEnvironmentRepository repo)
    {
        _repo = repo;
    }

    public Task<List<EnvironmentDefinition>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _repo.GetByTenantAsync(tenantId, cancellationToken);

    public Task<EnvironmentDefinition?> GetOneAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, id, cancellationToken);

    public async Task<EnvironmentDefinition> CreateAsync(Guid tenantId, string name, string type, string? description, bool isDefault, CancellationToken cancellationToken = default)
    {
        var env = new EnvironmentDefinition
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Type = string.IsNullOrWhiteSpace(type) ? "production" : type.Trim(),
            Description = description,
            IsDefault = isDefault
        };

        await _repo.AddAsync(env, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return env;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid id, string? name, string? type, string? description, bool? isDefault, CancellationToken cancellationToken = default)
    {
        var env = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (env is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) env.Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(type)) env.Type = type.Trim();
        if (description is not null) env.Description = description;
        if (isDefault is not null) env.IsDefault = isDefault.Value;

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var env = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (env is null) return false;
        await _repo.RemoveAsync(env, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
