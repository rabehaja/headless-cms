namespace ContentModels.Domain;

public class ContentModelSettings
{
    public bool EnableVersioning { get; set; }
    public bool EnableValidations { get; set; }
    public Dictionary<string, object?> Additional { get; set; } = new();
}
