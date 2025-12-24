# Delivery.Api

Delivery/preview API for published entries with caching and API key support.

## Highlights
- Serves published entries per tenant/content model/environment/locale.
- Supports `preview` to include unpublished entries.
- Cache headers (Cache-Control) and ETag middleware.
- Auth: JWT or `X-Delivery-Key`. `UseInMemory` available for tests.

## Configuration
- `ConnectionStrings:DeliveryDb` (Postgres), `UseInMemory` for tests.
- `Jwt:*` and `ApiKeys:Delivery`.

## Key Endpoints
- `GET /tenants/{tenantId}/content-models/{modelId}/entries?preview=false`
