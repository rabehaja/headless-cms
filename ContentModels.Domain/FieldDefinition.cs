namespace ContentModels.Domain;

public class FieldDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid ContentModelId { get; set; }
    public FieldType Type { get; set; }
    public string? Description { get; set; }
    public bool Required { get; set; }
    public string? GlobalFieldKey { get; set; }
    public string? TargetContentModel { get; set; }
    public FieldSettings Settings { get; set; } = new();
}
