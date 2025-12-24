using ContentModels.Domain;
using Microsoft.EntityFrameworkCore;

namespace Search.Api.Data;

public class SearchDbContext : DbContext
{
    public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options)
    {
    }

    public DbSet<TaxonomyGroup> Taxonomies => Set<TaxonomyGroup>();
    public DbSet<SearchIndexItem> Index => Set<SearchIndexItem>();
}
