# DSA API - Cloud-Native API Standard

This repository serves as a **production-ready reference implementation** of a .NET 10 API, demonstrating modern software engineering practices, "Shift-Left" security, and robust architectural patterns.

While the business logic focuses on Data Structures & Algorithms (DSA), the primary goal is to showcase **Enterprise Grade** delivery standards.

---

## 🏗 Architectural Patterns

### 1. Vertical Slice Architecture

Instead of the traditional layered approach (Controllers/Services/Repositories), code is organized by **Feature** (`Features/Sorting`).

- **Why:** Increases maintainability by keeping related code (API endpoints, Algorithms, DTOs, Business Logic) physically close. Reduces coupling between unrelated features.

### 2. Domain-Driven Design (DDD) Principles

- **Explicit Contracts (DTOs):** API models (`SortResult`) are decoupled from internal domain logic.
- **Polymorphism:** Algorithms implement strict interfaces (`ISortAlgorithm`), allowing runtime swapping of strategies (Strategy Pattern) without breaking consumers.

### 3. Modern .NET Standards (NET 10 Preview)

- **Minimal APIs:** Utilizing lightweight route builders with `TypedResults` for type-safe, high-performance HTTP responses.
- **Native OpenAPI:** using the built-in Microsoft OpenAPI generator for standard compliance without heavy third-party dependencies.

### 4. Operational Excellence

- **RFC 7807 Error Handling:** Global implementation of standard Problem Details for consistent client error parsing.
- **Split Health Probes:** Dedicated `/health/live` (Liveness) and `/health/ready` (Readiness) endpoints for zero-downtime deployments.
- **Resilience (Rate Limiting):** Implements **Partitioned Sliding Window** rate limiting (Token Bucket algorithm).
  - Uses `Microsoft.AspNetCore.RateLimiting` middleware.
  - Limits are partitioned by **IP Address** for anonymous users.
  - Configurable windows (default: 100 req/min).
  - Features smart bypass for `/health` checks to prevent self-DOS during load.
  - Production-ready `ForwardedHeadersMiddleware` support for Proxy/LoadBalancer environments.

---

## 🛡️ Security & DevOps (Shift-Left)

This project implements a **"Secure by Default"** pipeline, treating security as a quality gate, not an afterthought.

### CI/CD Pipeline (`.github/workflows/ci-pipeline.yml`)

The GitHub Actions pipeline is architected with strict dependencies:

1.  **SAST (Static Application Security Testing):**
    - **Trivy (Filesystem):** Scans for hardcoded secrets, misconfigurations, and vulnerable dependencies.
    - **CodeQL:** Performs deep semantic analysis of the C# code flow to detect injection vulnerabilities and logic flaws.
2.  **Automated Quality Gates:**
    - The build fails if _any_ unit or integration test fails.
    - The build fails if _critical_ vulnerabilities are detected.
3.  **SCA (Software Composition Analysis):**
    - The Docker container is scanned _before_ it is pushed to the registry to ensure the underlying OS (Debian/Alpine) is free of CVEs.
4.  **Secure Delivery:**
    - Artifacts are only pushed to **GitHub Container Registry (GHCR)** if _all_ previous security and quality gates pass.

### Smart Versioning & Release Strategy

This project uses a custom, dependency-free **Smart Release** strategy to automate semantic versioning and reduce registry noise.

1.  **Smart Release Gating:** The pipeline (`docker-delivery.yml`) analyzes commits and **only** releases a new container version if functional changes are detected.
    - **Artifacts Released:** If commits contain `feat`, `fix`, `perf`, or `refactor`.
    - **Release Skipped:** If commits are only `docs`, `chore`, `test`, or `ci`. (Saves CI minutes & storage).

2.  **Semantic Versioning:** Version numbers are calculated automatically based on [Conventional Commits](https://www.conventionalcommits.org/).

| Commit Type        | Impact    | Example                      | Result             |
| :----------------- | :-------- | :--------------------------- | :----------------- |
| `BREAKING CHANGE:` | **Major** | `feat!: drop support for v1` | `1.2.0` -> `2.0.0` |
| `feat:`            | **Minor** | `feat: add quicksort alg`    | `1.2.0` -> `1.3.0` |
| `fix:` / `perf:`   | **Patch** | `fix: off-by-one error`      | `1.2.1` -> `1.2.2` |
| `docs:` / `chore:` | **None**  | `docs: update readme`        | **No Release**     |

---

## 🧪 Testing Pyramid Strategy

We implement a full testing pyramid to ensure confidence at every level:

| Level           | Scope                  | Tech Stack                           | Purpose                                                                                                                                                       |
| --------------- | ---------------------- | ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Unit**        | `DSA.UnitTests`        | xUnit, `[ClassData]`                 | Validates algorithmic correctness (sorting logic) in total isolation.                                                                                         |
| **Integration** | `DSA.IntegrationTests` | `WebApplicationFactory`, NSubstitute | Validates API wiring, middleware, and correct DI configuration. Uses **advanced mocking** to simulate edge cases (e.g., forcing a 500 Internal Server Error). |
| **E2E (Smoke)** | `DSA.E2E`              | `HttpClient`, xUnit Traits           | Validates the deployed artifact is reachable and healthy (Black-box testing).                                                                                 |

---

## 🐳 Containerization

- **Multi-Stage Builds:** The `Dockerfile` uses distinct build and runtime stages.
- **Optimization:** The final image contains only the compiled binaries and the lightweight ASP.NET runtime (no SDKs), minimizing the attack surface and image size.
- **Non-Root Execution:** Designed to be run as a non-privileged user for security compliance.

---

## 🛠 Technology Stack

- **Framework:** .NET 10 (Preview)
- **Language:** C# 13 / 14 (Latest features)
- **Testing:** xUnit, NSubstitute (Mocking)
- **Container:** Docker / OCI Compliant
- **CI/CD:** GitHub Actions
- **Security Tools:** AquaSec Trivy, GitHub CodeQL

## 🤝 Community & Support

- **Contributing:** Please read our [Contribution Guidelines](CONTRIBUTING.md) before submitting a PR.
- **Security:** Found a vulnerability? See our [Security Policy](SECURITY.md).
- **Code of Conduct:** We pledge to foster an open and welcoming environment. Read our [Code of Conduct](CODE_OF_CONDUCT.md).

## 🚀 Getting Started

### Local Development

```bash
# Restore dependencies
dotnet restore

# Run Tests
dotnet test

# Run API
dotnet run --project src/DSA.Api/DSA.Api.csproj
```
