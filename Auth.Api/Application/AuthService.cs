using System.Security.Cryptography;
using System.Text;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;

namespace Auth.Api.Application;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserAccount> RegisterAsync(string email, string password, string role, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new UserAccount
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = HashPassword(password),
            Role = string.IsNullOrWhiteSpace(role) ? "viewer" : role,
            TenantId = tenantId
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<UserAccount?> ValidateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null) return null;

        var hashed = HashPassword(password);
        if (!string.Equals(hashed, user.PasswordHash, StringComparison.Ordinal))
        {
            return null;
        }

        return user;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}
