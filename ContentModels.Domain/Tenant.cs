namespace ContentModels.Domain;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public List<ContentModel> ContentModels { get; set; } = new();

    public ContentModel AddContentModel(string name, string? description, List<FieldDefinition> fields, ContentModelSettings settings)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Content model name is required.", nameof(name));
        }

        if (ContentModels.Any(m => string.Equals(m.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Content model with name '{name}' already exists in this tenant.");
        }

        var model = new ContentModel
        {
            Name = name.Trim(),
            Description = description,
            TenantId = Id,
            Settings = settings
        };

        foreach (var field in fields)
        {
            model.AddField(field);
        }

        ContentModels.Add(model);
        return model;
    }
}
