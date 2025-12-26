using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Tenants.Api.Application;

public class BranchService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IBranchRepository _branchRepository;

    public BranchService(ITenantRepository tenantRepository, IBranchRepository branchRepository)
    {
        _tenantRepository = tenantRepository;
        _branchRepository = branchRepository;
    }

    public async Task<List<Branch>> GetByTenantAsync(Guid organizationId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken);
        if (!exists) throw new InvalidOperationException("Tenant not found.");

        return await _branchRepository.GetByTenantAsync(tenantId, cancellationToken);
    }

    public async Task<Branch?> GetAsync(Guid organizationId, Guid tenantId, Guid branchId, CancellationToken cancellationToken = default)
    {
        var exists = await _tenantRepository.ExistsAsync(tenantId, organizationId, cancellationToken);
        if (!exists) return null;

        return await _branchRepository.GetAsync(tenantId, branchId, cancellationToken);
    }

    public async Task<Branch> CreateAsync(Guid organizationId, Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetAsync(tenantId, organizationId, cancellationToken);
        if (tenant is null) throw new InvalidOperationException("Tenant not found.");

        if (await _branchRepository.ExistsByNameAsync(tenantId, name.Trim(), cancellationToken))
        {
            throw new InvalidOperationException($"Branch '{name}' already exists in this tenant.");
        }

        var branch = new Branch { Name = name.Trim(), TenantId = tenantId };
        await _branchRepository.AddAsync(branch, cancellationToken);
        await _branchRepository.SaveChangesAsync(cancellationToken);
        return branch;
    }
}
