using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Organizations.Api.Data;

public class OrganizationsDbContext : DbContext
{
    public OrganizationsDbContext(DbContextOptions<OrganizationsDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
}
