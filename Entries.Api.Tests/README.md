# Entries.Api.Tests

API tests for Entries.Api.

## Notes
- Uses WebApplicationFactory with in-memory DB (`UseInMemory=true` via factory config).
- Default auth headers not set; tests assert unauthorized responses or can set headers explicitly.
- Expand with auth-happy-path CRUD when wiring auth headers.
