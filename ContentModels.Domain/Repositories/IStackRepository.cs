namespace ContentModels.Domain.Repositories;

public interface IStackRepository
{
    Task<Stack?> GetAsync(Guid stackId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid organizationId, string name, CancellationToken cancellationToken = default);
    Task AddAsync(Stack stack, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
