using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace ContentModels.Api.Application;

public class ContentModelService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IContentModelRepository _contentModelRepository;

    public ContentModelService(ITenantRepository tenantRepository, IContentModelRepository contentModelRepository)
    {
        _tenantRepository = tenantRepository;
        _contentModelRepository = contentModelRepository;
    }

    public async Task<List<ContentModel>> GetByBranchAsync(Guid organizationId, Guid tenantId, Guid branchId, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken);
        if (!exists) throw new InvalidOperationException("Tenant not found.");
        return await _contentModelRepository.GetByBranchAsync(tenantId, branchId, cancellationToken);
    }

    public async Task<ContentModel?> GetAsync(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken);
        if (!exists) return null;
        return await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
    }

    public async Task<ContentModel> CreateAsync(Guid organizationId, Guid tenantId, Guid branchId, string name, string? description, List<FieldDefinition> fields, ContentModelSettings settings, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetAsync(tenantId, organizationId, cancellationToken);
        if (tenant is null) throw new InvalidOperationException("Tenant not found.");

        if (await _contentModelRepository.ExistsByNameAsync(tenantId, branchId, name.Trim(), cancellationToken))
        {
            throw new InvalidOperationException($"Content model '{name}' already exists in this branch.");
        }

        var model = new ContentModel
        {
            Name = name.Trim(),
            Description = description,
            TenantId = tenantId,
            BranchId = branchId,
            Settings = settings
        };

        model.ReplaceFields(fields);

        await _contentModelRepository.AddAsync(model, cancellationToken);
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<bool> UpdateAsync(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, string? name, string? description, List<FieldDefinition>? fields, ContentModelSettings? settings, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;

        if (!await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var nameExists = await _contentModelRepository.ExistsByNameAsync(tenantId, branchId, name.Trim(), cancellationToken);
            if (nameExists && !string.Equals(model.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Content model '{name}' already exists in this branch.");
            }

            model.Name = name.Trim();
        }

        if (description is not null)
        {
            model.Description = description;
        }

        if (fields is not null)
        {
            model.ReplaceFields(fields);
        }

        if (settings is not null)
        {
            model.Settings = settings;
        }

        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSettingsAsync(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, ContentModelSettings settings, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;
        if (!await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken)) return false;

        model.Settings = settings;
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid organizationId, Guid tenantId, Guid branchId, Guid modelId, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;
        if (!await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken)) return false;

        await _contentModelRepository.RemoveAsync(model, cancellationToken);
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
