using System.Text.Json.Nodes;

namespace ContentModels.Domain;

public class AuditLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Actor { get; set; } = "unknown";
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid? ResourceId { get; set; }
    public Guid? StackId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string Source { get; set; } = string.Empty;
    public JsonObject Metadata { get; set; } = new();
}
