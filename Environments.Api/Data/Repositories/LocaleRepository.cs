using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Environments.Api.Data.Repositories;

public class LocaleRepository : ILocaleRepository
{
    private readonly EnvironmentsDbContext _db;

    public LocaleRepository(EnvironmentsDbContext db)
    {
        _db = db;
    }

    public Task<Locale?> GetAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default) =>
        _db.Locales.FirstOrDefaultAsync(l => l.Id == id && l.TenantId == tenantId && l.BranchId == branchId, cancellationToken);

    public Task<List<Locale>> GetByBranchAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _db.Locales.Where(l => l.TenantId == tenantId && l.BranchId == branchId).ToListAsync(cancellationToken);

    public async Task AddAsync(Locale locale, CancellationToken cancellationToken = default)
    {
        await _db.Locales.AddAsync(locale, cancellationToken);
    }

    public Task RemoveAsync(Locale locale, CancellationToken cancellationToken = default)
    {
        _db.Locales.Remove(locale);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
