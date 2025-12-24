namespace ContentModels.Domain;

public class SearchIndexItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid ContentModelId { get; set; }
    public Guid EntryId { get; set; }
    public string Locale { get; set; } = "en-us";
    public string Text { get; set; } = string.Empty;
    public DateTime IndexedAt { get; set; } = DateTime.UtcNow;
    public List<string> Taxonomies { get; set; } = new();
}
