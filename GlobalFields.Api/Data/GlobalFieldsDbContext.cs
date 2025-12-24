using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace GlobalFields.Api.Data;

public class GlobalFieldsDbContext : DbContext
{
    public GlobalFieldsDbContext(DbContextOptions<GlobalFieldsDbContext> options) : base(options)
    {
    }

    public DbSet<GlobalFieldDefinition> GlobalFields => Set<GlobalFieldDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GlobalFieldDefinition>()
            .OwnsOne(f => f.Settings, owned =>
            {
                owned.Property(s => s.Values).HasColumnType("jsonb");
            });
    }
}
