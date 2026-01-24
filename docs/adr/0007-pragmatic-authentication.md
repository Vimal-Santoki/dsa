# 7. Pragmatic Authentication with Secure Defaults

Date: 2026-01-25

## Status

Accepted

## Context

We are building an Enterprise-Grade API that requires security (Authorization). 
However, setting up a full-blown Identity Provider (IdP) like Azure AD B2C, Duende IdentityServer, or Auth0 adds significant operational complexity and cost during the initial development phases.

We faced a conflict between two requirements:
1.  **Velocity:** We need to implement features without being blocked by complex infrastructure setup.
2.  **Security:** We cannot ship an insecure API. We need "Principal-Grade" security patterns (Bearer Tokens, Claims, Policies) from Day 1.

We also needed to ensure that the application architecture adheres to the **Open/Closed Principle** and **Dependency Inversion Principle**, so that when we eventually swap the "Mock Auth" for "Real Auth", the Feature code (Endpoints) does not change.

## Decision

We decided to implement a **Pragmatic Authentication** strategy using a "Mock Issuer" pattern and "Secure by Default" policies.

### 1. The "Mock Issuer" Pattern
Instead of connecting to an external OIDC provider, the API acts as its own minimalistic IdP for development.
*   We implemented a `MockIdentityService` that generates valid, signed JWTs (HS256) locally.
*   This service is hidden behind an `IIdentityService` interface.

### 2. Vertical Slice Architecture for Auth
Authentication is treated as a **Common Concern** but structured like a Feature:
*   `src/DSA.Api/Common/Auth/` contains the logic.
*   It exposes `MapAuthEndpoints()` (for `/connect/token`) and `AddAuth()` (for DI).
*   It owns its own DTOs (`LoginRequest`, `TokenResponse`) and Configurations (`JwtSettings`).

### 3. Secure by Default (Global Fallback Policy)
We rejected the standard practice of adding `[Authorize]` attributes to every endpoint.
Instead, we configured a **Global Fallback Policy** in `AuthExtensions.cs`:
*   **Default:** All endpoints require an authenticated user.
*   **Exception:** Public endpoints (Health, Swagger, Login) must explicitly opt-out using `.AllowAnonymous()`.
This ensures that if a developer forgets to secure a new endpoint, it defaults to being secure (Closed), rather than open.

## Consequences

### Positive
*   **Zero External Dependencies:** Use cases run out-of-the-box (local dev, CI/CD, Docker) without needing an external IdP container.
*   **Testability:** Integration tests can easily "log in" by hitting the `/connect/token` endpoint, verifying the entire auth loop without mocking `HttpContext`.
*   **Architecture Protection:** The `Features/Sorting` code depends only on the User Principal. It does not know or care that the token came from a Mock Service.
*   **Security Posture:** New features cannot accidentally be exposed effectively eliminating "Broken Object Level Authorization" risks caused by forgetting attributes.

### Negative
*   **Maintenance:** We maintain a small amount of "Identity" code (Token generation) that is not core business logic.
*   **Client Complexity:** Clients (like the E2E tests) must perform a 2-step call (Login -> API) even in local development.

## Implementation Details

*   **Config:** `JwtSettings` in `appsettings.json`.
*   **Wiring:** `builder.AddAuth()` adds pure services *and* standard ASP.NET Core `AddAuthentication().AddJwtBearer()`.
*   **Pipeline:** `UseAuthentication()` and `UseAuthorization()` are explicitly placed before feature mapping.
