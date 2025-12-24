# Environments.Api

Manages environments and locales per tenant.

## Highlights
- CRUD for environments (production/staging/dev) with defaults.
- CRUD for locales with fallback/default flags.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:EnvironmentsDb`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/environments`
- `POST /tenants/{tenantId}/locales`
