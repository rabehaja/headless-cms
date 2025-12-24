namespace ContentModels.Domain;

public class UserAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "viewer";
    public bool Active { get; set; } = true;
}
