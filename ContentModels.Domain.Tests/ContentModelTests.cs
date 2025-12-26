using System;
using System.Collections.Generic;
using ContentModels.Domain;
using Xunit;

namespace ContentModels.Domain.Tests;

public class ContentModelTests
{
    [Fact]
    public void AddField_DisallowsUnknownType()
    {
        var model = new ContentModel();
        var field = new FieldDefinition { Name = "Bad", Type = (FieldType)999 };

        Assert.Throws<ArgumentException>(() => model.AddField(field));
    }

    [Fact]
    public void AddField_RequiresTargetForReference()
    {
        var model = new ContentModel();
        var field = new FieldDefinition { Name = "Ref", Type = FieldType.Reference };

        Assert.Throws<ArgumentException>(() => model.AddField(field));
    }

    [Fact]
    public void Tenant_AddContentModel_EnforcesUniqueName()
    {
        var tenant = new Tenant { Name = "Tenant", StackId = Guid.NewGuid() };
        var branch = tenant.AddBranch("main");
        branch.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings());

        Assert.Throws<InvalidOperationException>(() =>
            branch.AddContentModel("Article", null, new List<FieldDefinition>(), new ContentModelSettings()));
    }
}
