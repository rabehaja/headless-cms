using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Assets.Api.Data;

public class AssetsDbContext : DbContext
{
    public AssetsDbContext(DbContextOptions<AssetsDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();
}
