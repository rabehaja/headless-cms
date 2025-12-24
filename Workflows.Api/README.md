# Workflows.Api

Workflow definitions for entries/content models.

## Highlights
- Stores workflow steps with role associations.
- Emits webhook events on create/update/delete via WebhookNotifier.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:WorkflowsDb`, `Jwt:*`, `ApiKeys:Management`, `Webhooks:BaseUrl` to reach Webhooks API.

## Key Endpoints
- `POST /tenants/{tenantId}/workflows`
- `PUT /tenants/{tenantId}/workflows/{id}`
- `DELETE /tenants/{tenantId}/workflows/{id}`
