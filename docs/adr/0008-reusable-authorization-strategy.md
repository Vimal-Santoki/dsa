# 8. Reusable Authorization Strategy (Filtering)

Date: 2026-01-26

## Status

Accepted

## Context

After establishing "Pragmatic Authentication" (ADR 0007), we successfully identified *who* the user is. Now we must solve the problem of *what* they are allowed to do.

We identified several "Smells" in the initial authorization implementation attempts:
1.  **Coupling:** Function Handlers (Endpoints) were injecting `IAuthorizationService` and `ClaimsPrincipal`. This polluted the business logic with security concerns.
2.  **Magic Strings:** Permission names like `"Sorting.Execute"` were scattered across the codebase as raw strings.
3.  **Repetition:** Every endpoint had to manually write `if (!result.Succeeded) return Forbidden();`.
4.  **Leakage:** Returning specific error messages about *why* authorization failed (e.g., "Missing Requirement X") gives attackers information about the system's internal security model.

## Decision

We decided to implement a **Declarative Permission System** using ASP.NET Core `IEndpointFilter` and Compile-Time Constants.

### 1. The "Endpoint Filter" Pattern
We moved authorization logic *out* of the Endpoint handler and *into* the Middleware pipeline using `EndpointPermissionFilter`.
*   **What it does:** It intercepts the request *after* Authentication but *before* the Endpoint Logic.
*   **How it works:** It inspects the definition of the endpoint to see if it requires a permission. If the user lacks the permission, the filter short-circuits the pipeline with `403 Forbidden` or `404 Not Found`.
*   **Extension Method:** We introduced `.RequirePermission(string permission, string resource)` to make the syntax fluent and readable.

### 2. ABAC/RBAC Hybrid Model
Instead of checking for Roles (`User`, `Admin`), we check for **Capabilities** (Permissions).
*   **MockPermissionService:** Used to map User Identity -> Roles -> Permissions.
*   **Constants:** Created `AppPermissions` to centralize all permission strings (e.g., `AppPermissions.Sorting.Execute`). This eliminates "Magic String" typos and allows "Find All References" to see where protections are applied.

### 3. Strict Separation of Concerns
*   **The Endpoint:** Only knows how to sort data. It has no references to `ClaimsPrincipal`, `User`, or `Policies`.
*   **The Filter:** Only knows how to valid security. It has no reference to sorting algorithms.

### 4. Information Hiding
We strictly forbid the API from telling an unauthorized user *why* they failed.
*   **401 Unauthorized:** You are not logged in.
*   **403 Forbidden:** You are logged in, but you can't do this. (No further info).
*   **404 Not Found:** Use this when we want to hide the *existence* of a resource if the user doesn't have permission to see it.

## Consequences

### Positive
*   **Clean Code:** Feature endpoints dropped all Security Dependencies, making their method signatures cleaner (`RunSortAlgorithm` now only takes `algorithm` and `data`).
*   **Reusability:** The `EndpointPermissionFilter` can be applied to any future feature (Graph, Search, Trees) without duplicating logic.
*   **Maintainability:** Changing how we look up permissions (e.g., moving from In-Memory to SQL) only requires changing the `IamService`, not the 50 API endpoints.
*   **Type Safety:** Using `AppPermissions` constants ensures compile-time safety for security rules.

### Negative
*   **Complexity:** We added a layer of indirection (Filters) which can be harder for junior developers to debug compared to imperative `if/else` checks.
*   **Discovery:** Developers must know to look at the `.RequirePermission()` extension method to understand why a request is blocked, as it's not in the method body.
