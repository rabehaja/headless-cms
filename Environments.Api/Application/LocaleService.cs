using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Environments.Api.Application;

public class LocaleService
{
    private readonly ILocaleRepository _repo;

    public LocaleService(ILocaleRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Locale>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _repo.GetByTenantAsync(tenantId, cancellationToken);

    public Task<Locale?> GetOneAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, id, cancellationToken);

    public async Task<Locale> CreateAsync(Guid tenantId, string code, string name, string? fallback, bool isDefault, CancellationToken cancellationToken = default)
    {
        var locale = new Locale
        {
            TenantId = tenantId,
            Code = code.Trim().ToLowerInvariant(),
            Name = name.Trim(),
            Fallback = fallback?.Trim().ToLowerInvariant(),
            Default = isDefault
        };

        await _repo.AddAsync(locale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return locale;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid id, string? name, string? fallback, bool? isDefault, CancellationToken cancellationToken = default)
    {
        var locale = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (locale is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) locale.Name = name.Trim();
        if (fallback is not null) locale.Fallback = string.IsNullOrWhiteSpace(fallback) ? null : fallback.Trim().ToLowerInvariant();
        if (isDefault is not null) locale.Default = isDefault.Value;

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var locale = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (locale is null) return false;

        await _repo.RemoveAsync(locale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
