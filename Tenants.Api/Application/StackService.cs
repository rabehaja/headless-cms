using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Tenants.Api.Application;

public class StackService
{
    private readonly IStackRepository _stackRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public StackService(IStackRepository stackRepository, IOrganizationRepository organizationRepository)
    {
        _stackRepository = stackRepository;
        _organizationRepository = organizationRepository;
    }

    public Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _stackRepository.GetByOrganizationAsync(organizationId, cancellationToken);

    public Task<Stack?> GetAsync(Guid organizationId, Guid stackId, CancellationToken cancellationToken = default) =>
        _stackRepository.GetAsync(stackId, organizationId, cancellationToken);

    public async Task<Stack> CreateAsync(Guid organizationId, string name, CancellationToken cancellationToken = default)
    {
        var org = await _organizationRepository.GetAsync(organizationId, cancellationToken);
        if (org is null) throw new InvalidOperationException("Organization not found.");

        if (await _stackRepository.ExistsByNameAsync(organizationId, name.Trim(), cancellationToken))
        {
            throw new InvalidOperationException($"Stack '{name}' already exists for this organization.");
        }

        var stack = new Stack { Name = name.Trim(), OrganizationId = organizationId };
        await _stackRepository.AddAsync(stack, cancellationToken);
        await _stackRepository.SaveChangesAsync(cancellationToken);
        return stack;
    }
}
