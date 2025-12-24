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
        var tenant = new Tenant { Name = "Tenant" };
        tenant.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings());

        Assert.Throws<InvalidOperationException>(() =>
            tenant.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings()));
    }
}
