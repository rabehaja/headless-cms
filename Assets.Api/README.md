# Assets.Api

Tenant-scoped asset metadata service.

## Highlights
- CRUD for assets with filename, content type, size, URL, description.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:AssetsDb`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/assets`
- `GET /tenants/{tenantId}/assets`
