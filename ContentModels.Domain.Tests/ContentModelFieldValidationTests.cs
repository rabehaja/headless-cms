using System;
using System.Collections.Generic;
using ContentModels.Domain;
using Xunit;

namespace ContentModels.Domain.Tests;

public class ContentModelFieldValidationTests
{
    [Theory]
    [InlineData("", FieldType.SingleLineText)]
    [InlineData("   ", FieldType.Boolean)]
    public void AddField_Requires_Name(string name, FieldType type)
    {
        var model = new ContentModel();
        var field = new FieldDefinition { Name = name, Type = type };
        Assert.Throws<ArgumentException>(() => model.AddField(field));
    }

    [Fact]
    public void AddField_Rejects_UnsupportedType()
    {
        var model = new ContentModel();
        var field = new FieldDefinition { Name = "X", Type = (FieldType)999 };
        Assert.Throws<ArgumentException>(() => model.AddField(field));
    }

    [Fact]
    public void AddField_Rejects_Reference_Without_Target()
    {
        var model = new ContentModel();
        var field = new FieldDefinition { Name = "Ref", Type = FieldType.Reference };
        Assert.Throws<ArgumentException>(() => model.AddField(field));
    }

    [Fact]
    public void ReplaceFields_Replaces_All_With_Validation()
    {
        var model = new ContentModel();
        var fields = new List<FieldDefinition>
        {
            new() { Name = "Title", Type = FieldType.SingleLineText },
            new() { Name = "Body", Type = FieldType.MultiLineText }
        };

        model.ReplaceFields(fields);
        Assert.Equal(2, model.Fields.Count);
        Assert.Contains(model.Fields, f => f.Name == "Title");
    }
}
