using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly TenantsDbContext _db;

    public AuditLogRepository(TenantsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        await _db.AuditLogs.AddAsync(entry, cancellationToken);
    }

    public Task<List<AuditLogEntry>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default) =>
        _db.AuditLogs.OrderByDescending(a => a.Timestamp).Take(take).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
