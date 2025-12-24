# Webhooks.Api

Webhook subscriptions and delivery.

## Highlights
- CRUD webhook subscriptions with events, secret, max retries, active flag.
- Background dispatcher signs payloads (HMAC SHA256), retries with exponential backoff.
- Manual dispatch endpoint to enqueue events; Workflows API uses this for workflow events.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:WebhooksDb`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/webhooks`
- `POST /tenants/{tenantId}/webhooks/dispatch`
