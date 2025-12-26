using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace ContentModels.Api.Application;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IStackRepository _stackRepository;

    public TenantService(ITenantRepository tenantRepository, IStackRepository stackRepository)
    {
        _tenantRepository = tenantRepository;
        _stackRepository = stackRepository;
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
        return tenant;
    }
}
