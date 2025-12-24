# Content Models Microservices Suite

This repository contains a set of .NET 9 microservices that together model a Contentstack-like CMS. All services are wired for PostgreSQL by default, with optional in-memory providers for tests, and containerized via `docker-compose`. Major services:

- **ContentModels.Api**: Manage organizations, tenants, content models, standard fields, settings.
- **GlobalFields.Api**: Manage reusable global fields scoped per tenant.
- **Entries.Api**: Authoring service for entries with workflow states, scheduling, environments, locales, and taxonomies.
- **Delivery.Api**: Read-only delivery/preview API for published entries; supports API keys, caching headers, and ETags.
- **Environments.Api**: Manage environments and locales per tenant.
- **Roles.Api**: Roles and API keys (management/delivery).
- **Assets.Api**: Tenant-scoped assets metadata.
- **Auth.Api**: Basic user registration/login with JWT issuance (demo).
- **Workflows.Api**: Workflow definitions with webhook notifications.
- **Webhooks.Api**: Webhook subscriptions with signed delivery, retries, and event dispatch.
- **Search.Api**: Taxonomies and search index (Postgres + Elasticsearch fallback).
- **Tenants.Api / Organizations.Api**: Tenant/org CRUD (alternate services).

## Getting Started
1. Install .NET 9 SDK and Docker.
2. Run `docker-compose up --build` to start Postgres, Elasticsearch, and all APIs. Default ports:
   - ContentModels: 5001, GlobalFields: 5002, Entries: 5003, Assets: 5004, Auth: 5005, Tenants: 5006, Organizations: 5007, Environments: 5008, Roles: 5009, Workflows: 5010, Webhooks: 5011, Delivery: 5012, Search: 5013.
3. Connection strings live in each API’s `appsettings.json`; `docker-compose.yml` overrides with container hosts.
4. JWT keys/API keys are configured in `appsettings.json` (e.g., `ApiKeys:Management`, `ApiKeys:Delivery`); adjust before production.

## Tests & Coverage
- Tests are under `*.Tests` projects. Run `dotnet test /p:CollectCoverage=true`.
- Test factories set `UseInMemory=true` to avoid external DBs and provide in-memory auth handlers.

## Migrations
- Generate/apply EF Core migrations per service (e.g., ContentModels.Api) when changing schemas. Services support `UseInMemory` for testing.

## Search & Webhooks
- Search API uses Elasticsearch (via `Elastic__Uri`) with Postgres fallback.
- Webhook dispatcher signs payloads (HMAC SHA256) with subscription secret; retries with backoff; workflows emit webhook events.

## Key Endpoints (examples)
- Content models: `POST /organizations/{orgId}/tenants/{tenantId}/content-models`
- Entries (authoring): `POST /organizations/{orgId}/tenants/{tenantId}/content-models/{modelId}/entries`
- Delivery: `GET /tenants/{tenantId}/content-models/{modelId}/entries?preview=false` (requires delivery key)
- Standard fields: `GET /standard-fields`
- Workflows: `POST /tenants/{tenantId}/workflows`
- Webhooks: `POST /tenants/{tenantId}/webhooks`

## Security
- Management APIs accept JWT (role claims) or API keys (`X-Management-Key`).
- Delivery API accepts JWT or `X-Delivery-Key`.
- Webhooks use HMAC signatures; Auth API issues JWTs for demo only.
