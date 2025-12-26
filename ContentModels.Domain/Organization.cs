namespace ContentModels.Domain;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Tenant> Tenants { get; set; } = new();
    public List<Stack> Stacks { get; set; } = new();

    public Tenant AddTenant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tenant name is required.", nameof(name));
        }

        var tenant = new Tenant { Name = name.Trim(), OrganizationId = Id };
        tenant.AddBranch("main");
        Tenants.Add(tenant);
        return tenant;
    }

    public Stack AddStack(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Stack name is required.", nameof(name));
        }

        if (Stacks.Any(s => string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Stack with name '{name}' already exists in this organization.");
        }

        var stack = new Stack { Name = name.Trim(), OrganizationId = Id };
        Stacks.Add(stack);
        return stack;
    }
}
