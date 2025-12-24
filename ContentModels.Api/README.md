# ContentModels.Api

Manages organizations, tenants, content models, standard field types, and model settings.

## Highlights
- Supports standard field types (single-line, multiline, rich text JSON, date/time, file, group, link, modular block, taxonomy, boolean, reference, custom, global field).
- Tenant/org hierarchy; content models scoped per tenant.
- Settings per content model; CRUD endpoints for orgs/tenants/content models/standard fields.
- Auth: JWT (role claim) or `X-Management-Key`. In tests, `UseInMemory` toggles an in-memory DB.

## Configuration
- `ConnectionStrings:ContentModelsDb` for Postgres (default).
- `UseInMemory=true` to switch to in-memory DB (tests).
- `Jwt:*` and `ApiKeys:Management` for auth.

## Key Endpoints
- `POST /organizations` create org
- `POST /organizations/{orgId}/tenants` create tenant
- `POST /organizations/{orgId}/tenants/{tenantId}/content-models`
- `GET /standard-fields`
