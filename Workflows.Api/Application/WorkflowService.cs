using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Workflows.Api.Services;

namespace Workflows.Api.Application;

public class WorkflowService
{
    private readonly IWorkflowRepository _repo;
    private readonly IWebhookNotifier _webhookNotifier;

    public WorkflowService(IWorkflowRepository repo, IWebhookNotifier webhookNotifier)
    {
        _repo = repo;
        _webhookNotifier = webhookNotifier;
    }

    public Task<List<Workflow>> GetAsync(Guid tenantId, Guid branchId, CancellationToken cancellationToken = default) =>
        _repo.GetByBranchAsync(tenantId, branchId, cancellationToken);

    public Task<Workflow?> GetOneAsync(Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, branchId, id, cancellationToken);

    public async Task<Workflow> CreateAsync(Guid stackId, Guid tenantId, Guid branchId, string name, List<WorkflowStep> steps, CancellationToken cancellationToken = default)
    {
        var wf = new Workflow
        {
            TenantId = tenantId,
            BranchId = branchId,
            Name = name.Trim(),
            Steps = steps
        };

        await _repo.AddAsync(wf, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(stackId, tenantId, branchId, "workflow.created", new { workflowId = wf.Id, wf.Name, wf.Steps }, cancellationToken);
        return wf;
    }

    public async Task<bool> UpdateAsync(Guid stackId, Guid tenantId, Guid branchId, Guid id, string? name, List<WorkflowStep>? steps, CancellationToken cancellationToken = default)
    {
        var wf = await _repo.GetAsync(tenantId, branchId, id, cancellationToken);
        if (wf is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) wf.Name = name.Trim();
        if (steps is not null) wf.Steps = steps;

        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(stackId, tenantId, branchId, "workflow.updated", new { workflowId = wf.Id, wf.Name, wf.Steps }, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid stackId, Guid tenantId, Guid branchId, Guid id, CancellationToken cancellationToken = default)
    {
        var wf = await _repo.GetAsync(tenantId, branchId, id, cancellationToken);
        if (wf is null) return false;

        await _repo.RemoveAsync(wf, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(stackId, tenantId, branchId, "workflow.deleted", new { workflowId = wf.Id }, cancellationToken);
        return true;
    }
}
