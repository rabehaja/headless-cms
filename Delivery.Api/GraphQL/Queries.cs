using ContentModels.Domain;
using Delivery.Api.Application;

namespace Delivery.Api.GraphQL;

public class Query
{
    public Task<List<DeliveryEntry>> Entries(
        [Service] DeliveryService service,
        Guid tenantId,
        Guid branchId,
        Guid modelId,
        Guid? environmentId,
        string? locale,
        bool preview = false,
        CancellationToken cancellationToken = default)
    {
        return service.GetAsync(tenantId, branchId, modelId, environmentId, locale, preview, cancellationToken);
    }

    public Task<DeliveryEntry?> Entry(
        [Service] DeliveryService service,
        Guid tenantId,
        Guid branchId,
        Guid modelId,
        Guid entryId,
        bool preview = false,
        CancellationToken cancellationToken = default)
    {
        return service.GetOneAsync(tenantId, branchId, modelId, entryId, preview, cancellationToken);
    }
}
