# GlobalFields.Api

Manages reusable global field definitions per tenant.

## Highlights
- CRUD global fields with type, required flag, settings (jsonb).
- Auth: JWT or management key (shared with other management services).

## Configuration
- `ConnectionStrings:GlobalFieldsDb`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/global-fields`
- `GET /tenants/{tenantId}/global-fields`
