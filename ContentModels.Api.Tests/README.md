# ContentModels.Api.Tests

API tests for ContentModels.Api using WebApplicationFactory.

## Notes
- Factory injects in-memory config and DB (`UseInMemory=true`) and registers a test auth scheme (`Test`).
- Management API key is set to `management-key` in the factory.
- Tests can add `X-Management-Key` or rely on the test auth scheme; expand with CRUD happy-path cases as needed.
