using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Environments.Api.Data;

public class EnvironmentsDbContext : DbContext
{
    public EnvironmentsDbContext(DbContextOptions<EnvironmentsDbContext> options) : base(options)
    {
    }

    public DbSet<EnvironmentDefinition> Environments => Set<EnvironmentDefinition>();
    public DbSet<Locale> Locales => Set<Locale>();
}
