using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Moq;
using Workflows.Api.Application;
using Workflows.Api.Services;
using Xunit;

namespace Workflows.Api.Tests;

public class WorkflowServiceTests
{
    [Fact]
    public async Task Create_Notifies_Webhook()
    {
        var repo = new Mock<IWorkflowRepository>();
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var notifier = new Mock<IWebhookNotifier>();
        notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var svc = new WorkflowService(repo.Object, notifier.Object);

        var wf = await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Flow", new List<WorkflowStep> { new() { Name = "Draft" } });

        Assert.Equal("Flow", wf.Name);
        notifier.Verify(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), "workflow.created", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_Notifies_Webhook()
    {
        var wf = new Workflow { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), BranchId = Guid.NewGuid(), Name = "Flow", Steps = new List<WorkflowStep>() };
        var repo = new Mock<IWorkflowRepository>();
        repo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(wf);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var notifier = new Mock<IWebhookNotifier>();
        notifier.Setup(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var svc = new WorkflowService(repo.Object, notifier.Object);

        var updated = await svc.UpdateAsync(Guid.NewGuid(), wf.TenantId, wf.BranchId, wf.Id, "New", new List<WorkflowStep> { new() { Name = "Draft" } });
        Assert.True(updated);
        notifier.Verify(n => n.NotifyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), "workflow.updated", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
