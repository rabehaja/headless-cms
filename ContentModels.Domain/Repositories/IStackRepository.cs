namespace ContentModels.Domain.Repositories;

public interface IStackRepository
{
    Task<Stack?> GetAsync(Guid stackId, CancellationToken cancellationToken = default);
    Task<List<Stack>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Stack stack, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
