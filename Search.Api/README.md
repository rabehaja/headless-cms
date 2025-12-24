# Search.Api

Taxonomy and search indexing service.

## Highlights
- Taxonomy groups/terms CRUD per tenant.
- Search index items with content model, entry, locale, and taxonomy facets.
- Elasticsearch-backed search (via `Elastic__Uri`/`Elastic__Index`) with Postgres fallback.
- Auth: JWT or management key.

## Configuration
- `ConnectionStrings:SearchDb`, `Elastic__Uri`, `Elastic__Index`, `Jwt:*`, `ApiKeys:Management`.

## Key Endpoints
- `POST /tenants/{tenantId}/taxonomies`
- `POST /tenants/{tenantId}/search/index`
- `GET /tenants/{tenantId}/search?q=...`
