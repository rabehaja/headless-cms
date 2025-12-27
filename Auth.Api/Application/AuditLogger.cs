using System.Security.Claims;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Auth.Api.Application;

public interface IAuditLogger
{
    Task AppendAsync(string action, string resourceType, Guid? resourceId, object? metadata = null, CancellationToken cancellationToken = default);
}

public class AuditLogger : IAuditLogger
{
    private readonly IAuditLogRepository _repo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogger(IAuditLogRepository repo, IHttpContextAccessor httpContextAccessor)
    {
        _repo = repo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task AppendAsync(string action, string resourceType, Guid? resourceId, object? metadata = null, CancellationToken cancellationToken = default)
    {
        var actor = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name
            ?? "anonymous";

        var entry = new AuditLogEntry
        {
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Actor = actor,
            Source = "Auth.Api",
            Metadata = metadata is null ? new() : System.Text.Json.JsonSerializer.SerializeToNode(metadata)!.AsObject()
        };

        await _repo.AddAsync(entry, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
