using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Organizations.Api.Application;

public class OrganizationService
{
    private readonly IOrganizationRepository _repository;

    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Organization>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _repository.GetAllAsync(cancellationToken);

    public Task<Organization?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _repository.GetAsync(id, cancellationToken);

    public async Task<Organization> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var org = new Organization { Name = name.Trim() };
        await _repository.AddAsync(org, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return org;
    }
}
