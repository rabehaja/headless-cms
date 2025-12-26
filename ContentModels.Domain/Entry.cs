namespace ContentModels.Domain;

public class Entry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
    public Guid ContentModelId { get; set; }
    public Guid EnvironmentId { get; set; }
    public string Locale { get; set; } = "en-us";
    public EntryState State { get; set; } = EntryState.Draft;
    public bool Published { get; set; }
    public List<Guid> TaxonomyIds { get; set; } = new();
    public int Version { get; set; } = 1;
    public DateTime? ScheduledPublishAt { get; set; }
    public DateTime? ScheduledUnpublishAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object?> Data { get; set; } = new();
}
