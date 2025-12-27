using System;
using System.Threading;
using System.Threading.Tasks;
using Auth.Api.Application;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Moq;
using Xunit;

namespace Auth.Api.Tests;

public class AuthServiceTests
{
    private class InMemoryUserRepo : IUserRepository
    {
        public UserAccount? User;

        public Task AddAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            User = user;
            return Task.CompletedTask;
        }

        public Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(User != null && User.Id == id ? User : null);

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(User != null && User.Email == email.ToLower() ? User : null);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public async Task Register_Then_Validate_Succeeds()
    {
        var repo = new InMemoryUserRepo();
        var audit = new Mock<IAuditLogger>();
        var svc = new AuthService(repo, audit.Object);

        var user = await svc.RegisterAsync("test@example.com", "password", "editor", null);
        Assert.Equal("editor", user.Role);

        var validated = await svc.ValidateAsync("test@example.com", "password");
        Assert.NotNull(validated);
        Assert.Equal(user.Id, validated!.Id);
    }

    [Fact]
    public async Task Validate_Fails_With_WrongPassword()
    {
        var repo = new InMemoryUserRepo();
        var audit = new Mock<IAuditLogger>();
        var svc = new AuthService(repo, audit.Object);
        await svc.RegisterAsync("test@example.com", "password", "editor", null);

        var validated = await svc.ValidateAsync("test@example.com", "wrong");
        Assert.Null(validated);
    }
}
