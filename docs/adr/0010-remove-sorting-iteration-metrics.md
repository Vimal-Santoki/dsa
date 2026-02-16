# 10. Remove Sorting Iteration Metrics

Date: 2026-02-04

## Status

Accepted

## Context

The initial design of the `ISortAlgorithm` interface included a return type of `int` for the `Sort` method, intended to return the number of "iterations" or "swaps" performed by the algorithm. This was primarily for educational purposes to compare algorithm complexity.

However, as we scaled the implementation to handle larger datasets and enterprise-grade concurrency, several critical issues emerged:

1.  **Integer Overflow**: For $O(N^2)$ algorithms (like Bubble Sort) running on large datasets (e.g., 100,000+ items), the number of operations easily exceeds `int.MaxValue` (2.1 billion), leading to meaningless negative numbers.
2.  **Concurrency Risks**: To track iterations, implementations often used a class-level field (e.g., `_iterations`). Since `ISortAlgorithm` implementations are registered as Singletons or Scoped services, this introduced race conditions where concurrent requests would corrupt each other's counters.
3.  **Performance Overhead**: Incrementing a counter inside the tightest inner loop of an algorithm adds measurable CPU overhead, negatively impacting the raw performance we are trying to measure.
4.  **Metric irrelevance**: "Iterations" is an ambiguous metric that does not compare well across different classes of algorithms (e.g., comparison sorts vs. non-comparison sorts).

## Decision

We have decided to **remove the iteration count** from the `ISortAlgorithm` interface and the public API.

*   The `Sort` method signature changes from `int Sort(int[])` to `void Sort(int[])`.
*   The `Iterations` field is removed from the `SortResult` API response.
*   We will rely on **OpenTelemetry** (Latency, CPU time) for all performance monitoring, which provides actionable, real-world metrics.

## Consequences

### Positive
*   **Thread Safety**: Sorting services are now stateless and thread-safe by default, allowing them to be safely registered as Singletons.
*   **Correctness**: We eliminate the risk of integer overflows on large datasets.
*   **Performance**: Removal of the counter increment in the inner loop improves raw sorting speed.
*   **Clarity**: The API contract is now focused on the outcome (sorting) rather than implementation details.

### Negative
*   **Breaking Change**: Clients referencing the `iterations` property in the API response will find it missing or missing data. This feature is retired.
