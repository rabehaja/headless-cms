using System;
using System.Collections.Generic;
using ContentModels.Domain;
using Xunit;

namespace ContentModels.Domain.Tests;

public class WorkflowTests
{
    [Fact]
    public void Workflow_Can_Add_Steps()
    {
        var wf = new Workflow
        {
            TenantId = Guid.NewGuid(),
            Name = "Publish Flow",
            Steps = new List<WorkflowStep>
            {
                new() { Name = "Draft", Roles = new List<string> { "author" } },
                new() { Name = "Review", Roles = new List<string> { "editor" } }
            }
        };

        Assert.Equal(2, wf.Steps.Count);
        Assert.Contains(wf.Steps, s => s.Name == "Review");
    }
}
