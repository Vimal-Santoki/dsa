# 4. Testing Strategy

Date: 2026-01-23

## Status

Accepted

## Context

In many implementations, "Unit Tests" rely heavily on mocking every dependency (Repositories, Services), which results in brittle tests that are tightly coupled to implementation details. Refactoring implementation often breaks tests even if behavior hasn't changed.

We need a testing strategy that gives us high confidence in deployment with minimal maintenance cost.

## Decision

We will implement a **Test Pyramid** with a strong preference for **Sociable Integration Tests** (Integration) over **Solitary Unit Tests** (Mock-heavy).

1.  **Strict Unit Tests**: Only for pure domain logic (algorithms) where inputs/outputs are deterministic. No I/O.
2.  **Integration Tests**: The default for features. Uses `WebApplicationFactory` to spin up the in-memory SUT (System Under Test).
    - **External Dependencies**: Mocked/Stubbed only at the infrastructure edge (e.g., FileSystem, 3rd party APIs).
    - **Internal Dependencies**: Real implementations are used (Sociable).
3.  **E2E Tests**: Black-box testing against a deployed (or dockerized) environment.

## Consequences

### Positive

- **Refactoring Safety**: We can change internal class structures without breaking tests, as long as the API contract remains valid.
- **Realism**: Tests run against the actual DI container and configuration, catching wiring issues early.

### Negative

- **Speed**: Slower than pure unit tests (though `WebApplicationFactory` is quite fast).
- **Complexity**: Setup requires managing `IServiceCollection` and potentially test containers in the future.
