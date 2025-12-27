using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Tenants.Api.Application;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IStackRepository _stackRepository;
    private readonly IAuditLogger _audit;

    public TenantService(ITenantRepository tenantRepository, IStackRepository stackRepository, IAuditLogger audit)
    {
        _tenantRepository = tenantRepository;
        _stackRepository = stackRepository;
        _audit = audit;
    }

    public Task<List<Tenant>> GetByStackAsync(Guid stackId, CancellationToken cancellationToken = default) =>
        _tenantRepository.GetByStackAsync(stackId, cancellationToken);

    public Task<Tenant?> GetAsync(Guid stackId, Guid tenantId, CancellationToken cancellationToken = default) =>
        _tenantRepository.GetAsync(tenantId, stackId, cancellationToken);

    public async Task<Tenant> CreateAsync(Guid stackId, string name, CancellationToken cancellationToken = default)
    {
        var stack = await _stackRepository.GetAsync(stackId, cancellationToken);
        if (stack is null) throw new InvalidOperationException("Stack not found.");

        var tenant = stack.AddTenant(name);
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);
        await _audit.AppendAsync("tenant.create", nameof(Tenant), tenant.Id, stackId, tenant.Id, null, new { name }, cancellationToken);
        return tenant;
    }
}
