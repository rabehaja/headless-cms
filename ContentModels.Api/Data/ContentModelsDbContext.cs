using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContentModels.Api.Data;

public class ContentModelsDbContext : DbContext
{
    public ContentModelsDbContext(DbContextOptions<ContentModelsDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ContentModel> ContentModels => Set<ContentModel>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Tenants)
            .WithOne()
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.ContentModels)
            .WithOne()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ContentModel>()
            .HasMany(c => c.Fields)
            .WithOne()
            .HasForeignKey(f => f.ContentModelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FieldDefinition>()
            .OwnsOne(f => f.Settings, owned =>
            {
                owned.Property(s => s.Values).HasColumnType("jsonb");
            });

        modelBuilder.Entity<ContentModel>()
            .OwnsOne(c => c.Settings, settings =>
            {
                settings.Property(s => s.EnableValidations);
                settings.Property(s => s.EnableVersioning);
                settings.Property(s => s.Additional).HasColumnType("jsonb");
            });
    }
}
