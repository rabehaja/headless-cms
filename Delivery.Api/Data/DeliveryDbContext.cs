using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Data;

public class DeliveryDbContext : DbContext
{
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options)
    {
    }

    public DbSet<DeliveryEntry> Entries => Set<DeliveryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryEntry>()
            .Property(e => e.Data)
            .HasColumnType("jsonb");

        modelBuilder.Entity<DeliveryEntry>()
            .Property(e => e.TaxonomyIds)
            .HasColumnType("jsonb");
    }
}
