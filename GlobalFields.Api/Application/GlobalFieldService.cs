using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace GlobalFields.Api.Application;

public class GlobalFieldService
{
    private readonly IGlobalFieldRepository _repository;

    public GlobalFieldService(IGlobalFieldRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GlobalFieldDefinition>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _repository.GetByBranchAsync(tenantId, branchId, cancellationToken);

    public Task<GlobalFieldDefinition?> GetAsync(Guid tenantId, Guid branchId, Guid fieldId, CancellationToken cancellationToken = default) =>
        _repository.GetAsync(tenantId, branchId, fieldId, cancellationToken);

    public async Task<GlobalFieldDefinition> CreateAsync(Guid tenantId, Guid branchId, string key, string name, FieldType type, bool required, FieldSettings settings, CancellationToken cancellationToken = default)
    {
        if (!StandardFields.Types.Contains(type))
        {
            throw new ArgumentException($"Field type {type} is not allowed.");
        }

        var field = new GlobalFieldDefinition
        {
            Key = key.Trim(),
            Name = name.Trim(),
            Type = type,
            Required = required,
            Settings = settings,
            TenantId = tenantId,
            BranchId = branchId
        };

        await _repository.AddAsync(field, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return field;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid branchId, Guid fieldId, string? key, string? name, FieldType? type, bool? required, FieldSettings? settings, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetAsync(tenantId, branchId, fieldId, cancellationToken);
        if (existing is null) return false;

        if (type is not null && !StandardFields.Types.Contains(type.Value))
        {
            throw new ArgumentException($"Field type {type} is not allowed.");
        }

        if (!string.IsNullOrWhiteSpace(key)) existing.Key = key.Trim();
        if (!string.IsNullOrWhiteSpace(name)) existing.Name = name.Trim();
        if (type is not null) existing.Type = type.Value;
        if (required is not null) existing.Required = required.Value;
        if (settings is not null) existing.Settings = settings;

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid branchId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetAsync(tenantId, branchId, fieldId, cancellationToken);
        if (existing is null) return false;

        await _repository.RemoveAsync(existing, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
