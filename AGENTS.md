# AGENTS.md

## Repository layout

- `src/EtherSharp` - main runtime library (`net10.0`).
- `src/EtherSharp.Generator` - Roslyn incremental source generator (`netstandard2.0`), bundled via the `EtherSharp` package analyzer assets.
- `src/EtherSharp.ERC` - ERC package built on top of generator outputs.
- `tests` - xUnit test project (`tests/EtherSharp.Tests.csproj`).
- `bench` - BenchmarkDotNet project (`bench/EtherSharp.Bench.csproj`).
- `.editorconfig` - style, naming, and formatting source of truth.

### Lint / style enforcement

- Build includes analyzer/style enforcement, but `dotnet format` is required to enforce non-silent suggestion rules.
- Enforce all non-silent `.editorconfig` diagnostics:
- `dotnet format --verify-no-changes --severity info`
- Reason: `--severity info` includes suggestion/warning/error and excludes silent.

## Generated code policy

- `src/EtherSharp` uses `.tt` templates that emit checked-in `.cs` files.
- Source generator emits compile-time `*.generated.cs`.
- Fix source templates/writers, not generated outputs.

## EditorConfig enforcement (all non-silent rules)

- Enforce all non-slient style guidelines from the .editorconfig

## Completion checklist for agents

- Apply smallest safe change.
- Run minimum necessary build/test; run full `dotnet test` for generator or ABI-surface changes.
- Run `dotnet format --verify-no-changes --severity info` before finalizing.
- Ensure all modified t4 templates have been re-evaluated 
