# Containerized CI Runner Strategy

Date: 2026-02-17

## Status

Accepted

## Context

We identified a significant "developer experience" gap where automated test suites (Linters, Unit Tests, Integration Tests) could pass locally on a developer's machine but fail in the GitHub Actions pipeline. This was caused by:
1.  **Environment Drift:** Different OS versions (Windows vs Linux), missing tools (`yamllint`, `hadolint`), or different SDK patch versions.
2.  **Implicit Dependencies:** Tests that relied on globally installed tools that were not pinned.
3.  **Context Bloat:** Docker builds failing because implicit `.dockerignore` rules allowed unwanted files into the build context.

We needed a strategy to ensure "If it works on my machine, it works in CI" without forcing developers to maintain complex local environments.

## Decision

We have decided to adopt a **"Containerized Task Runner"** approach for our CI lifecycle.

1.  **Taskfile as the Source of Truth:**
    We use [Task](https://taskfile.dev/) to define all build, lint, and test commands (`Taskfile.yml`). This replaces raw shell scripts and ensures commands are identical across environments.

2.  **Dedicated CI Container (`tests/Dockerfile.ci`):**
    We created a specialized Docker image that acts as our build agent.
    -   **Base:** `mcr.microsoft.com/dotnet/sdk:10.0`
    -   **Tools:** Installs `task`, `yamllint`, `hadolint`, `git`, and `curl` via explicit RUN commands.
    -   **Purist Context:** We use explicit `COPY` instructions (rejecting `COPY . .`) to prevent context pollution and ensure layer caching efficiency.

3.  **Execution via Docker Compose (`docker-compose.ci.yml`):**
    We use `task docker-ci` to execute the entire pipeline inside this container.
    -   Command: `docker compose run --rm ci-runner`
    -   Volumes: Mounts `./TestResults` to extract XML reports to the host.
    -   Network: Runs in isolation (no dependency on host network).

4.  **Pre-Push Hook Strategy:**
    Instead of a heavy pre-commit hook (which slows down save-points), we implemented a **Git Pre-Push Hook**.
    -   **Trigger:** `git push`
    -   **Action:** Runs `task docker-ci` silently.
    -   **Outcome:** If the containerized tests fail, the push is rejected.

## Consequences

### Positive

-   **Environment Parity:** The test environment is identical for every developer and the CI server. "Start from scratch" is now `docker compose build`.
-   **Zero Local Config:** Developers do not need to install `yamllint` or `hadolint` explicitly; they just need Docker.
-   **Reproducibility:** A failure in CI can be reproduced locally by running `task docker-ci`.
-   **Cleanliness:** The `ci-runner` container is ephemeral (`--rm`) and does not clutter the developer's machine.

### Negative

-   **Execution Speed:** Running tests inside a container is slower than running directly on the host due to container spin-up and file system mounting overhead.
-   **Docker Dependency:** Developers *must* have Docker Desktop (or equivalent) running to commit/push code.

### Risks

-   **Bypass:** Developers may bypass the pre-push hook (`git push --no-verify`) if the build time becomes excessive, defeating the purpose of the gate.
