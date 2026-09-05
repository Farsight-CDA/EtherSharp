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
- Never manually edit generated files, only ever touch their templates and run code generation after.

## EditorConfig enforcement (all non-silent rules)

- Enforce all non-slient style guidelines from the .editorconfig

Important style guides to remember:
- For expression-bodied methods with a single-line declaration, place the body on the next line.
- When a method declaration spans multiple lines, place the final closing `)` on its own line.
- For a multi-line expression-bodied method declaration, place `=>` on the same line as the final closing `)`.
- When a function call's arguments span multiple lines, place its closing `)` on a separate line from the final argument.

## Benchmarks

- When running benchmarks only run the ones relevant to your change and default to using very short runs unless specified differently.

## Completion checklist for agents

- Apply smallest safe change.
- Run minimum necessary build/test; run full `dotnet test` for generator or ABI-surface changes.
- Run `dotnet format --verify-no-changes --severity info` before finalizing.
- Ensure all modified t4 templates have been re-evaluated 
