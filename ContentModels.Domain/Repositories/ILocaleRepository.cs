namespace ContentModels.Domain.Repositories;

public interface ILocaleRepository
{
    Task<Locale?> GetAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default);
    Task<List<Locale>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default);
    Task AddAsync(Locale locale, CancellationToken cancellationToken = default);
    Task RemoveAsync(Locale locale, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
