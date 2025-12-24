namespace ContentModels.Domain;

public class WebhookSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public bool Active { get; set; } = true;
    public string Secret { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
}
