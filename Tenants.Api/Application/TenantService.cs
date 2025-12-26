using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Tenants.Api.Application;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IStackRepository _stackRepository;

    public TenantService(ITenantRepository tenantRepository, IOrganizationRepository organizationRepository, IStackRepository stackRepository)
    {
        _tenantRepository = tenantRepository;
        _organizationRepository = organizationRepository;
        _stackRepository = stackRepository;
    }

    public Task<List<Tenant>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _tenantRepository.GetByOrganizationAsync(organizationId, cancellationToken);

    public Task<List<Tenant>> GetByStackAsync(Guid organizationId, Guid stackId, CancellationToken cancellationToken = default) =>
        _tenantRepository.GetByStackAsync(organizationId, stackId, cancellationToken);

    public Task<Tenant?> GetAsync(Guid organizationId, Guid tenantId, CancellationToken cancellationToken = default) =>
        _tenantRepository.GetAsync(tenantId, organizationId, cancellationToken);

    public async Task<Tenant> CreateAsync(Guid organizationId, string name, Guid? stackId = null, CancellationToken cancellationToken = default)
    {
        var org = await _organizationRepository.GetAsync(organizationId, cancellationToken);
        if (org is null) throw new InvalidOperationException("Organization not found.");

        if (stackId is not null)
        {
            var stack = await _stackRepository.GetAsync(stackId.Value, organizationId, cancellationToken);
            if (stack is null) throw new InvalidOperationException("Stack not found.");
        }

        var tenant = org.AddTenant(name);
        tenant.StackId = stackId;
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);
        return tenant;
    }
}
