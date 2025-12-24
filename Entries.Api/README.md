# Entries.Api

Authoring service for entries tied to content models, tenants, environments, locales, and taxonomies.

## Highlights
- Entry state: Draft, InReview, Scheduled, Published, Unpublished.
- Scheduling (publish/unpublish timestamps), versioning, timestamps.
- Environment + locale + taxonomy IDs per entry.
- CRUD + publish/unpublish + scheduling endpoints.
- Auth: JWT or management key; `UseInMemory` for tests.

## Configuration
- `ConnectionStrings:EntriesDb` (Postgres), `UseInMemory` for in-memory DB in tests.
- `Jwt:*` and management API key.

## Key Endpoints
- `POST /organizations/{orgId}/tenants/{tenantId}/content-models/{modelId}/entries`
- `PUT /organizations/{orgId}/tenants/{tenantId}/content-models/{modelId}/entries/{entryId}`
- `POST /.../{entryId}/publish`, `POST /.../{entryId}/unpublish`, `POST /.../{entryId}/schedule`
