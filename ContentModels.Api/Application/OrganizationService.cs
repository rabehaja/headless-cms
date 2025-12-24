using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace ContentModels.Api.Application;

public class OrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _organizationRepository.GetAllAsync(cancellationToken);

    public Task<Organization?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _organizationRepository.GetAsync(id, cancellationToken);

    public async Task<Organization> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var org = new Organization { Name = name.Trim() };
        await _organizationRepository.AddAsync(org, cancellationToken);
        await _organizationRepository.SaveChangesAsync(cancellationToken);
        return org;
    }
}
