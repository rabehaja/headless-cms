namespace ContentModels.Domain.Repositories;

public interface IWorkflowRepository
{
    Task<Workflow?> GetAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default);
    Task<List<Workflow>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(Workflow workflow, CancellationToken cancellationToken = default);
    Task RemoveAsync(Workflow workflow, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
