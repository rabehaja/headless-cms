namespace ContentModels.Domain;

public class ContentModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<FieldDefinition> Fields { get; set; } = new();
    public ContentModelSettings Settings { get; set; } = new();

    public void AddField(FieldDefinition field)
    {
        if (!StandardFields.Types.Contains(field.Type))
        {
            throw new ArgumentException($"Field type {field.Type} is not allowed.");
        }

        if (string.IsNullOrWhiteSpace(field.Name))
        {
            throw new ArgumentException("Field name is required.");
        }

        if (field.Type == FieldType.Reference && string.IsNullOrWhiteSpace(field.TargetContentModel))
        {
            throw new ArgumentException("Reference field must specify TargetContentModel.");
        }

        field.Settings ??= new FieldSettings();
        field.ContentModelId = Id;
        Fields.Add(field);
    }

    public void ReplaceFields(IEnumerable<FieldDefinition> fields)
    {
        Fields.Clear();
        foreach (var field in fields)
        {
            AddField(field);
        }
    }
}
