# 3. Strict Compiler Governance

Date: 2026-01-23

## Status

Accepted

## Context

Software quality often degrades over time as technical debt accumulates. Warnings are ignored, and inconsistent coding styles creep in. For a "Principal-grade" repository, we want to ensure that the codebase remains pristine and that quality is enforced by tooling, not just human review.

## Decision

We will enforce a "Zero Warning" policy and strict code analysis via `Directory.Build.props`.

1.  **TreatWarningsAsErrors**: Set to `true`. The build fails if there is a single warning.
2.  **AnalysisLevel**: Set to `latest-all`. We opt-in to the most aggressive set of .NET analyzers.
3.  **EnforceCodeStyleInBuild**: Code style violations (e.g., naming conventions, spacing) break the build.

Configuration in `Directory.Build.props`:

```xml
<PropertyGroup>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

## Consequences

### Positive

- **Quality**: The codebase maintains a measurable high standard automatically.
- **Consistency**: No debates about code style in PRs; the compiler is the arbiter.
- **Maintenance**: Preventing warnings from entering the main branch prevents "broken window" syndrome.

### Negative

- **Velocity**: Initial development is slower as the compiler rejects valid but "messy" code.
- **Friction**: Prototyping requires disabling strictness or ignoring errors temporarily.
