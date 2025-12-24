using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Roles.Api.Application;

public class RoleService
{
    private readonly IRoleRepository _roles;

    public RoleService(IRoleRepository roles)
    {
        _roles = roles;
    }

    public Task<List<Role>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _roles.GetByTenantAsync(tenantId, cancellationToken);

    public Task<Role?> GetOneAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default) =>
        _roles.GetAsync(tenantId, roleId, cancellationToken);

    public async Task<Role> CreateAsync(Guid tenantId, string name, List<string> permissions, CancellationToken cancellationToken = default)
    {
        var role = new Role
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Permissions = permissions
        };

        await _roles.AddAsync(role, cancellationToken);
        await _roles.SaveChangesAsync(cancellationToken);
        return role;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid roleId, string? name, List<string>? permissions, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetAsync(tenantId, roleId, cancellationToken);
        if (role is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) role.Name = name.Trim();
        if (permissions is not null) role.Permissions = permissions;

        await _roles.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetAsync(tenantId, roleId, cancellationToken);
        if (role is null) return false;
        await _roles.RemoveAsync(role, cancellationToken);
        await _roles.SaveChangesAsync(cancellationToken);
        return true;
    }
}
