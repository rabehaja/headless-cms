using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace ContentModels.Api.Application;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public TenantService(ITenantRepository tenantRepository, IOrganizationRepository organizationRepository)
    {
        _tenantRepository = tenantRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<List<Tenant>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _tenantRepository.GetByOrganizationAsync(organizationId, cancellationToken);
    }

    public async Task<Tenant?> GetAsync(Guid organizationId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _tenantRepository.GetAsync(tenantId, organizationId, cancellationToken);
    }

    public async Task<Tenant> CreateAsync(Guid organizationId, string name, CancellationToken cancellationToken = default)
    {
        var org = await _organizationRepository.GetAsync(organizationId, cancellationToken);
        if (org is null) throw new InvalidOperationException("Organization not found.");

        var tenant = org.AddTenant(name);
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);
        return tenant;
    }
}
