namespace ContentModels.Domain;

public class TaxonomyGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaxonomyTerm> Terms { get; set; } = new();
}

public class TaxonomyTerm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? ParentId { get; set; }
}
