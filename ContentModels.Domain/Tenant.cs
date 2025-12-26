namespace ContentModels.Domain;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public Guid? StackId { get; set; }
    public List<Branch> Branches { get; set; } = new();

    public Branch AddBranch(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Branch name is required.", nameof(name));
        }

        if (Branches.Any(b => string.Equals(b.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Branch with name '{name}' already exists in this tenant.");
        }

        var branch = new Branch { Name = name.Trim(), TenantId = Id };
        Branches.Add(branch);
        return branch;
    }
}
