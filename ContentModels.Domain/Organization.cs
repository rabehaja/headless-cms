namespace ContentModels.Domain;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Tenant> Tenants { get; set; } = new();

    public Tenant AddTenant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tenant name is required.", nameof(name));
        }

        var tenant = new Tenant { Name = name.Trim(), OrganizationId = Id };
        Tenants.Add(tenant);
        return tenant;
    }
}
