namespace ContentModels.Domain;

public class EnvironmentDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "production"; // production, staging, development
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}
