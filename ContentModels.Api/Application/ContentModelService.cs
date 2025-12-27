using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace ContentModels.Api.Application;

public class ContentModelService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IContentModelRepository _contentModelRepository;
    private readonly IBranchRepository _branchRepository;

    public ContentModelService(ITenantRepository tenantRepository, IContentModelRepository contentModelRepository, IBranchRepository branchRepository)
    {
        _tenantRepository = tenantRepository;
        _contentModelRepository = contentModelRepository;
        _branchRepository = branchRepository;
    }

    public async Task<List<ContentModel>> GetByBranchAsync(Guid stackId, Guid tenantId, Guid branchId, bool inherit = false, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken);
        if (!exists) throw new InvalidOperationException("Tenant not found.");
        if (!inherit)
        {
            return await _contentModelRepository.GetByBranchAsync(tenantId, branchId, cancellationToken);
        }

        var branchChain = await BuildBranchChainAsync(tenantId, branchId, cancellationToken);
        var merged = new Dictionary<Guid, ContentModel>();
        foreach (var b in branchChain)
        {
            var models = await _contentModelRepository.GetByBranchAsync(tenantId, b.Id, cancellationToken);
            foreach (var model in models)
            {
                merged[model.Id] = model;
            }
        }

        return merged.Values.ToList();
    }

    public async Task<ContentModel?> GetAsync(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, bool inherit = false, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken);
        if (!exists) return null;
        if (!inherit)
        {
            return await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        }

        var branchChain = await BuildBranchChainAsync(tenantId, branchId, cancellationToken);
        foreach (var b in branchChain)
        {
            var model = await _contentModelRepository.GetAsync(tenantId, b.Id, modelId, cancellationToken);
            if (model is not null) return model;
        }

        return null;
    }

    public async Task<ContentModel> CreateAsync(Guid stackId, Guid tenantId, Guid branchId, string name, string? description, List<FieldDefinition> fields, ContentModelSettings settings, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetAsync(tenantId, stackId, cancellationToken);
        if (tenant is null) throw new InvalidOperationException("Tenant not found.");
        await EnsureBranchWritable(tenantId, branchId, cancellationToken);

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
            OriginBranchId = branchId,
            Settings = settings
        };

        model.ReplaceFields(fields);

        await _contentModelRepository.AddAsync(model, cancellationToken);
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<bool> UpdateAsync(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, string? name, string? description, List<FieldDefinition>? fields, ContentModelSettings? settings, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;

        if (!await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken))
        {
            return false;
        }
        await EnsureBranchWritable(tenantId, branchId, cancellationToken);

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

        model.Version += 1;
        model.UpdatedAt = DateTime.UtcNow;
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSettingsAsync(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, ContentModelSettings settings, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;
        if (!await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken)) return false;
        await EnsureBranchWritable(tenantId, branchId, cancellationToken);

        model.Settings = settings;
        model.Version += 1;
        model.UpdatedAt = DateTime.UtcNow;
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid stackId, Guid tenantId, Guid branchId, Guid modelId, CancellationToken cancellationToken = default)
    {
        var model = await _contentModelRepository.GetAsync(tenantId, branchId, modelId, cancellationToken);
        if (model is null) return false;
        if (!await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken)) return false;
        await EnsureBranchWritable(tenantId, branchId, cancellationToken);

        await _contentModelRepository.RemoveAsync(model, cancellationToken);
        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ContentModelMergePreview> PreviewMergeAsync(Guid stackId, Guid tenantId, Guid sourceBranchId, Guid targetBranchId, CancellationToken cancellationToken = default)
    {
        if (!await _tenantRepository.ExistsAsync(tenantId, stackId, cancellationToken))
        {
            throw new InvalidOperationException("Tenant not found.");
        }
        await EnsureNotDefault(tenantId, targetBranchId, cancellationToken);

        var source = await _contentModelRepository.GetByBranchAsync(tenantId, sourceBranchId, cancellationToken);
        var target = await _contentModelRepository.GetByBranchAsync(tenantId, targetBranchId, cancellationToken);

        var targetMap = target.ToDictionary(m => m.Id, m => m);
        var adds = new List<ContentModel>();
        var updates = new List<ContentModel>();
        var conflicts = new List<ContentModelDiff>();

        foreach (var model in source)
        {
            if (!targetMap.TryGetValue(model.Id, out var tgt))
            {
                adds.Add(CloneForBranch(model, targetBranchId));
                continue;
            }

            if (model.Version > tgt.Version)
            {
                updates.Add(CloneForBranch(model, targetBranchId, tgt.Id, tgt.Version));
            }
            else if (model.Version < tgt.Version)
            {
                conflicts.Add(new ContentModelDiff(model.Id, model.Name, model.Version, tgt.Version, true, "Target is newer"));
            }
        }

        return new ContentModelMergePreview(adds, updates, conflicts);
    }

    public async Task<bool> ApplyMergeAsync(Guid stackId, Guid tenantId, Guid sourceBranchId, Guid targetBranchId, CancellationToken cancellationToken = default)
    {
        var preview = await PreviewMergeAsync(stackId, tenantId, sourceBranchId, targetBranchId, cancellationToken);
        if (preview.Conflicts.Any()) return false;

        foreach (var add in preview.Adds)
        {
            await _contentModelRepository.AddAsync(add, cancellationToken);
        }

        foreach (var update in preview.Updates)
        {
            var existing = await _contentModelRepository.GetAsync(tenantId, targetBranchId, update.Id, cancellationToken);
            if (existing is null) continue;
            existing.Name = update.Name;
            existing.Description = update.Description;
            existing.Settings = update.Settings;
            existing.ReplaceFields(update.Fields);
            existing.Version = update.Version + 1;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.OriginBranchId ??= sourceBranchId;
        }

        await _contentModelRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ContentModel CloneForBranch(ContentModel model, Guid targetBranchId, Guid? existingId = null, int? baseVersion = null)
    {
        var clone = new ContentModel
        {
            Id = existingId ?? Guid.NewGuid(),
            TenantId = model.TenantId,
            BranchId = targetBranchId,
            OriginBranchId = model.BranchId,
            Name = model.Name,
            Description = model.Description,
            Settings = model.Settings,
            Version = (baseVersion ?? model.Version) + 1,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var field in model.Fields)
        {
            var f = new FieldDefinition
            {
                Name = field.Name,
                Type = field.Type,
                Required = field.Required,
                TargetContentModel = field.TargetContentModel,
                Settings = field.Settings
            };
            clone.AddField(f);
        }

        return clone;
    }

    private async Task<List<Branch>> BuildBranchChainAsync(Guid tenantId, Guid leafBranchId, CancellationToken cancellationToken)
    {
        var chain = new List<Branch>();
        var current = await _branchRepository.GetAsync(tenantId, leafBranchId, cancellationToken);
        while (current is not null)
        {
            chain.Add(current);
            if (current.ParentBranchId is null) break;
            current = await _branchRepository.GetAsync(tenantId, current.ParentBranchId.Value, cancellationToken);
        }

        chain.Reverse();
        return chain;
    }

    private async Task EnsureNotDefault(Guid tenantId, Guid branchId, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetAsync(tenantId, branchId, cancellationToken);
        if (branch is not null && branch.IsDefault)
        {
            throw new InvalidOperationException("Cannot merge into default branch directly.");
        }
    }

    private async Task EnsureBranchWritable(Guid tenantId, Guid branchId, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetAsync(tenantId, branchId, cancellationToken);
        if (branch is null) throw new InvalidOperationException("Branch not found.");
        if (branch.State == BranchState.Protected || branch.State == BranchState.Archived)
        {
            throw new InvalidOperationException("Branch is protected or archived.");
        }
    }
}
