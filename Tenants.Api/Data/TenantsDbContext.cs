using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Api.Data;

public class TenantsDbContext : DbContext
{
    public TenantsDbContext(DbContextOptions<TenantsDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Stack> Stacks => Set<Stack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Tenants)
            .WithOne()
            .HasForeignKey(t => t.OrganizationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Branches)
            .WithOne()
            .HasForeignKey(b => b.TenantId)
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
    }
}
