using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Tenants.Api.Application;

public class StackService
{
    private readonly IStackRepository _stackRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IAuditLogger _audit;

    public StackService(IStackRepository stackRepository, IOrganizationRepository organizationRepository, IAuditLogger audit)
    {
        _stackRepository = stackRepository;
        _organizationRepository = organizationRepository;
        _audit = audit;
    }

    public Task<List<Stack>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _stackRepository.GetAllAsync(cancellationToken);

    public Task<List<Stack>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        _stackRepository.GetByOrganizationAsync(organizationId, cancellationToken);

    public Task<Stack?> GetAsync(Guid stackId, CancellationToken cancellationToken = default) =>
        _stackRepository.GetAsync(stackId, cancellationToken);

    public async Task<Stack> CreateAsync(string name, Guid? organizationId, CancellationToken cancellationToken = default)
    {
        if (organizationId is not null)
        {
            var org = await _organizationRepository.GetAsync(organizationId.Value, cancellationToken);
            if (org is null) throw new InvalidOperationException("Organization not found.");
        }

        if (await _stackRepository.ExistsByNameAsync(name.Trim(), cancellationToken))
        {
            throw new InvalidOperationException($"Stack '{name}' already exists.");
        }

        var stack = new Stack { Name = name.Trim(), OrganizationId = organizationId };
        await _stackRepository.AddAsync(stack, cancellationToken);
        await _stackRepository.SaveChangesAsync(cancellationToken);
        await _audit.AppendAsync("stack.create", nameof(Stack), stack.Id, stack.Id, null, null, new { name, organizationId }, cancellationToken);
        return stack;
    }
}
