namespace ContentModels.Domain.Repositories;

public interface IBranchRepository
{
    Task<Branch?> GetAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task<List<Branch>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task AddAsync(Branch branch, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
