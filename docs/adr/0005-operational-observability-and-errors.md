# 5. Operational Observability and Standardized Errors

Date: 2026-01-23

## Status

Accepted

## Context

Production systems require consistent behavior when failing. 
1. Clients (Mobile/Web) need a predictable error schema to parse messages. 
2. Orchestrators (Kubernetes) need specific endpoints to know when to restart a container (Liveness) vs when to stop sending traffic (Readiness).

## Decision

### 1. Global Error Handling via RFC 7807

We will strictly use the **IETF RFC 7807 (Problem Details)** standard for all API errors.
*   **Mechanism**: `IExceptionHandler` middleware.
*   **Schema**: JSON response containing `type`, `title`, `status`, `detail`, and `instance`.
*   **Why**: Removes ambiguity. Frontends don't have to guess if an error is `message` or `errorMessage` or `error.msg`.

### 2. Split Health Checks

We will expose dedicated probes for specific lifecycle stages:
*   `/health/live`: Checks if the process is running. (Failure = Restart Pod).
*   `/health/ready`: Checks if dependencies (DB, Cache) are connected. (Failure = Stop Traffic).

## Consequences

*   **Positive**: Instant compatibility with standard tools (Datadog, K8s, Uptime Kuma) and generated clients.
*   **Negative**: Slight overhead in configuring separate probes vs a single simple `/ping` endpoint.
