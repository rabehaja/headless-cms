namespace ContentModels.Domain;

public class Locale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
    public string Code { get; set; } = "en-us";
    public string Name { get; set; } = "English";
    public string? Fallback { get; set; }
    public bool Default { get; set; }
}
