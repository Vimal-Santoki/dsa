# 9. OpenTelemetry-First Observability Strategy

Date: 2026-01-29

## Status

Accepted

## Context

As the system scales, localized logging and simple health checks (ADR-0005) are insufficient for diagnosing complex distributed issues. We face specific challenges:
1.  **Invisible Failures**: Rate-limited requests (429s) were previously invisible in metrics, hiding "silent" outages.
2.  **Signal Disconnect**: Metrics showed *that* a spike occurred, but provided no link to *why* (Trace ID) or *what happened* (Logs), leading to long MTTR (Mean Time To Recovery).
3.  **Cost vs. Visibility**: Tracing 100% of requests is prohibitively expensive in production, yet 0% sampling misses critical errors.
4.  **Signal Correlation**: Finding the needle in the haystack (exemplar traces) from aggregate heatmaps was manual and error-prone.

## Decision

We will adopt a "Principal-Grade" **OpenTelemetry-First** strategy that prioritizes signal correlation and production readiness.

### 1. Unified OpenTelemetry SDK
We standardize on the `.NET OpenTelemetry` SDK for all signals (Logs, Metrics, Traces). We replace vendor-specific agents with the OTLP protocol.
- **Collector Pattern**: Deployment of an OpenTelemetry Collector sidecar/gateway to decouple implementation from backend storage (initially Grafana/Prometheus/Tempo/Loki).

### 2. Signal Correlation & Exemplars
We enforce tight coupling between signal types to enable "Metadata-Driven Navigation":
- **Metrics to Traces**: We enable **OpenMetrics Exemplars**. Every high-latency bucket in a heatmap must carry a `TraceID`, appearing as a visible "dot" allowing one-click jumps to the slow transaction.
- **Traces to Logs**: We inject `TraceID` and `SpanID` into all log records. We utilize **Loki Structured Metadata** to index `service.name` and `trace_id`, enabling bidirectional linking without expensive full-text search.

### 3. Production Sampling Strategy
To balance visibility with cost:
- **Head-Based Sampling**: We adopt `TraceIdRatioBasedSampler` with a default ratio of **0.1 (10%)**.
- **Overrides**: Critical paths or specific headers can force 100% sampling via dynamic configuration if needed in the future.

### 4. Rate Limiting Observability
We expressly instrument `Microsoft.AspNetCore.RateLimiting` to ensure dropped traffic is visible:
- **Activity Tagging**: We explicitly tag `http.response.status_code = 429` on the current Activity *before* the middleware short-circuits the pipeline.
- **Meter Recording**: We assert that 429s must appear in the `http.server.request.duration` metrics.

## Consequences

### Positive
- **Drastically Reduced MTTR**: Navigation from "Alert" -> "Metric Spike" -> "Exemplar Trace" -> "Correlated Logs" is seamless and requires no query writing.
- **Cost Efficiency**: 10% sampling reduces storage costs by 90% while statistically retaining enough error signals (as errors are often not rare during incidents).
- **Vendor Neutrality**: The OTLP standard allows switching backends (e.g., to Azure Monitor or Datadog) without code changes.

### Negative
- **Local Complexity**: Running the full stack (Collector, Prometheus, Tempo, Loki) in development requires Docker resources.
- **Configuration Overhead**: Correctly mapping OTLP attributes (e.g., `service.name` vs `service_name`) in Grafana Datasources is brittle and requires precise YAML configuration.
