namespace ContentModels.Domain;

public class GlobalFieldDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public FieldType Type { get; set; }
    public bool Required { get; set; }
    public FieldSettings Settings { get; set; } = new();
}
