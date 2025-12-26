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
    public DbSet<Stack> Stacks => Set<Stack>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<ContentModel> ContentModels => Set<ContentModel>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Tenants)
            .WithOne()
            .HasForeignKey(t => t.OrganizationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Stacks)
            .WithOne()
            .HasForeignKey(s => s.OrganizationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Stack>()
            .HasMany(s => s.Tenants)
            .WithOne()
            .HasForeignKey(t => t.StackId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Branches)
            .WithOne()
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Branch>()
            .HasMany(b => b.ContentModels)
            .WithOne()
            .HasForeignKey(c => c.BranchId)
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
