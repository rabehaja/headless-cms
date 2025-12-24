using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Workflows.Api.Data;

public class WorkflowsDbContext : DbContext
{
    public WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow> Workflows => Set<Workflow>();
}
