using System;
using System.Collections.Generic;
using ContentModels.Domain;
using Xunit;

namespace ContentModels.Domain.Tests;

public class TenantContentModelTests
{
    [Fact]
    public void Tenant_AddContentModel_Throws_On_Duplicate()
    {
        var tenant = new Tenant { Name = "Tenant", StackId = Guid.NewGuid() };
        var branch = tenant.AddBranch("main");
        branch.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings());

        Assert.Throws<InvalidOperationException>(() =>
            branch.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings()));
    }
}
