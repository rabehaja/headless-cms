# Auth.Api

Simple auth service for demo purposes.

## Highlights
- Register/login with email/password; hashes with SHA256.
- Issues JWT tokens containing role and tenant claims.
- Not production-ready; replace hashing/token logic for real deployments.

## Configuration
- `ConnectionStrings:AuthDb`, `Jwt:*` (key, issuer, audience).

## Key Endpoints
- `POST /auth/register`
- `POST /auth/login`
