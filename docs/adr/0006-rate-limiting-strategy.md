# 6. Rate Limiting Strategy

Date: 2026-01-24

## Status

Accepted

## Context

The DSA Learning API is designed to be potentially exposed as a public service or deployed within a shared cluster. Without protection, the API is vulnerable to:
1.  **Denial of Service (DoS):** Malicious actors flooding the Sorting algorithms (which are CPU intensive).
2.  **Noisy Neighbor Iteffects:** One consumer monopolizing resources.
3.  **Brute Force/Scraping:** Automated tools extracting data aggressively.

We need a mechanism to control traffic flow that is "Production Grade," meaning it must handle:
-   Reverse Proxies (Load Balancers/Kubernetes Ingress).
-   Distributed clients.
-   Health Check monitoring (which should not be blocked).

## Decision

We will implement **Partitioned Rate Limiting** using the native ASP.NET Core middleware (`Microsoft.AspNetCore.RateLimiting`).

### 1. Algorithm: Sliding Window
We selected **Sliding Window** over Fixed Window or Token Bucket.
-   **Why:** Fixed Window allows "Bursts" at the boundary (e.g., 100 reqs at 59s, 100 reqs at 61s = 200 reqs in 2 seconds). Sliding Window divides the time into segments, providing a smoother traffic distribution and preventing boundary abuse.
-   **Configuration:** 100 Requests / 60 Seconds (Default).

### 2. Partition Strategy: IP Address (Anonymous)
Since authentication is not yet implemented, we partition statistics by **Client IP Address**.
-   **Challenge:** In Docker/Kubernetes, the `RemoteIpAddress` is often the Load Balancer's IP.
-   **Solution:** We enabled `ForwardedHeadersMiddleware`. This middleware trusts the `X-Forwarded-For` header from the upstream ingress to resolve the *real* client IP.
-   **Security Note:** `KnownProxies.Clear()` and `KnownNetworks.Clear()` are used to trust the immediate container network, addressing the dynamic nature of container IPs. This assumes the container is deployed within a private network perimeter (e.g., K8s Cluster, Azure VNet).

### 3. Bypass Logic: Health Checks
We explicitly disable the limiter for `/health/*` endpoints.
-   **Reasoning:** Kubernetes Liveness/Readiness probes poll frequently. Blocking these probes due to false-positive rate limiting would cause the orchestration platform to kill and restart the healthy container (Cascading Failure).

### 4. Configuration Management
We reject "Magic Numbers." All limits are defined in a strongly-typed `RateLimitingSettings` class, bindable to `appsettings.json` or Environment Variables (`RateLimiting__PermitLimit`).

## Consequences

### Positive
-   **Resilience:** API is protected from simple flooding attacks.
-   **Stability:** CPU-intensive sorting operations are capped.
-   **Observability:** Health probes remain reliable under load.

### Negative
-   **NAT Impact:** Multiple legitimate users behind a single corporate NAT/VPN will share the same IP quota. This is an accepted trade-off for anonymous APIs.
-   **Proxy Complexity:** Incorrect ingress configuration (failing to pass `X-Forwarded-For`) will result in all users sharing one quota (the LB's IP), effectively blocking the API.

## Future Considerations
-   **User Partitioning:** Once Authentication (Phase 4) is implemented, we will introduce a "Hybrid Partitioning" strategy:
    -   Authenticated Users -> Partition by `User.Identity.Name` (Higher Limit).
    -   Anonymous Users -> Fallback to IP (Lower Limit).
