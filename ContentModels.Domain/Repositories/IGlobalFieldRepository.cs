namespace ContentModels.Domain.Repositories;

public interface IGlobalFieldRepository
{
    Task<GlobalFieldDefinition?> GetAsync(Guid tenantId, Guid branchId, Guid fieldId, CancellationToken cancellationToken = default);
    Task<List<GlobalFieldDefinition>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(GlobalFieldDefinition field, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(GlobalFieldDefinition field, CancellationToken cancellationToken = default);
}
