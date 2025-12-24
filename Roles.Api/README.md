# Roles.Api

Manages roles and API keys per tenant.

## Highlights
- Role CRUD with permission lists.
- API key CRUD (management/delivery types), active flag.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:RolesDb`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/roles`
- `POST /tenants/{tenantId}/api-keys`
