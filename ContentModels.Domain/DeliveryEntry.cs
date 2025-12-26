namespace ContentModels.Domain;

public class DeliveryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
    public Guid ContentModelId { get; set; }
    public Guid EnvironmentId { get; set; }
    public string Locale { get; set; } = "en-us";
    public bool Published { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<Guid> TaxonomyIds { get; set; } = new();
    public Dictionary<string, object?> Data { get; set; } = new();
}
