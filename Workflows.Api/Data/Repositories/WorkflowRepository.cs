using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Workflows.Api.Data.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly WorkflowsDbContext _db;

    public WorkflowRepository(WorkflowsDbContext db)
    {
        _db = db;
    }

    public Task<Workflow?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _db.Workflows.FirstOrDefaultAsync(w => w.Id == id && w.TenantId == tenantId, cancellationToken);

    public Task<List<Workflow>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _db.Workflows.Where(w => w.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task AddAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        await _db.Workflows.AddAsync(workflow, cancellationToken);
    }

    public Task RemoveAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        _db.Workflows.Remove(workflow);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}
