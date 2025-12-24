using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Roles.Api.Data;

public class RolesDbContext : DbContext
{
    public RolesDbContext(DbContextOptions<RolesDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
}
