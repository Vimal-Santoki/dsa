# 2. Vertical Slice Architecture

Date: 2026-01-23

## Status

Accepted

## Context

Traditional "Clean" or "Onion" architectures (Controller -> UI -> Biz -> Data layers) often lead to scattered domain logic and high coupling between disparate features. Modifying a single feature often requires touching multiple layers across the entire application, leading to "shotgun surgery."

As we are building a high-performance DSA API, we need an architecture that promotes:

1. High cohesion within features (Sorting, Searching, etc.).
2. Low coupling between features.
3. Easy discoverability of related code.

## Decision

We will use **Vertical Slice Architecture**.

Instead of organizing by technical concerns (Layers), we will organize by functional concerns (Features).
All code related to a specific feature (API endpoints, Application Logic, Domain Models, DTOs) will reside in a single folder under `Features/<FeatureName>`.

Structure:

```
Features/
    Sorting/
        Api/        # Endpoints (REPR pattern)
        Algorithms/ # Domain Logic
        Dto/        # Data Transfer
```

## Consequences

### Positive

- **Cohesion**: Related code is physically co-located.
- **Evolution**: Features can evolve independently (e.g., Sorting applies strict performance rules, while Admin features might prioritize maintainability).
- **Testing**: Integration tests target specific slices rather than mocking entire layers.

### Negative

- **Duplication**: Some cross-cutting logic might be duplicated if not carefully managed (though Shared Kernel can mitigate this).
- **Learning Curve**: Developers coming from strict Layered Architecture backgrounds may initially struggle with the lack of "Services" and "Repositories."
