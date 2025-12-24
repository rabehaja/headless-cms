using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Search.Api.Application;

public class TaxonomyService
{
    private readonly ITaxonomyRepository _repo;

    public TaxonomyService(ITaxonomyRepository repo)
    {
        _repo = repo;
    }

    public Task<List<TaxonomyGroup>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _repo.GetByTenantAsync(tenantId, cancellationToken);

    public Task<TaxonomyGroup?> GetOneAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, id, cancellationToken);

    public async Task<TaxonomyGroup> CreateAsync(Guid tenantId, string name, List<TaxonomyTerm> terms, CancellationToken cancellationToken = default)
    {
        var group = new TaxonomyGroup
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Terms = terms
        };

        await _repo.AddAsync(group, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return group;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid id, string? name, List<TaxonomyTerm>? terms, CancellationToken cancellationToken = default)
    {
        var group = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (group is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) group.Name = name.Trim();
        if (terms is not null) group.Terms = terms;

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var group = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (group is null) return false;

        await _repo.RemoveAsync(group, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
