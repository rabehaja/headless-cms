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

    public Task<List<Workflow>> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        _repo.GetByTenantAsync(tenantId, cancellationToken);

    public Task<Workflow?> GetOneAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _repo.GetAsync(tenantId, id, cancellationToken);

    public async Task<Workflow> CreateAsync(Guid tenantId, string name, List<WorkflowStep> steps, CancellationToken cancellationToken = default)
    {
        var wf = new Workflow
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Steps = steps
        };

        await _repo.AddAsync(wf, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(tenantId, "workflow.created", new { workflowId = wf.Id, wf.Name, wf.Steps }, cancellationToken);
        return wf;
    }

    public async Task<bool> UpdateAsync(Guid tenantId, Guid id, string? name, List<WorkflowStep>? steps, CancellationToken cancellationToken = default)
    {
        var wf = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (wf is null) return false;

        if (!string.IsNullOrWhiteSpace(name)) wf.Name = name.Trim();
        if (steps is not null) wf.Steps = steps;

        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(tenantId, "workflow.updated", new { workflowId = wf.Id, wf.Name, wf.Steps }, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var wf = await _repo.GetAsync(tenantId, id, cancellationToken);
        if (wf is null) return false;

        await _repo.RemoveAsync(wf, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        await _webhookNotifier.NotifyAsync(tenantId, "workflow.deleted", new { workflowId = wf.Id }, cancellationToken);
        return true;
    }
}
