using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Entries.Api.Data;

public class EntriesDbContext : DbContext
{
    public EntriesDbContext(DbContextOptions<EntriesDbContext> options) : base(options)
    {
    }

    public DbSet<Entry> Entries => Set<Entry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entry>()
            .Property(e => e.Data)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Entry>()
            .Property(e => e.TaxonomyIds)
            .HasColumnType("jsonb");
    }
}
