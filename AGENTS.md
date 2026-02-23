# AGENTS.md

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

Important style guides to remember:
- Place the body of expression body methods onto a new line 

## Completion checklist for agents

- Apply smallest safe change.
- Run minimum necessary build/test; run full `dotnet test` for generator or ABI-surface changes.
- Run `dotnet format --verify-no-changes --severity info` before finalizing.
- Ensure all modified t4 templates have been re-evaluated 
