# AGENTS.md

Operational guide for agentic coding tools in `G:\Farsight\EtherSharp`.

## Scope

- Applies to the whole repository.
- Keep changes minimal, targeted, and pattern-consistent.
- Do not edit generated outputs directly unless explicitly requested.

## Repository layout

- `src/EtherSharp` - main runtime library (`net10.0`).
- `src/EtherSharp.Generator` - Roslyn incremental source generator (`netstandard2.0`).
- `src/EtherSharp.ERC` - ERC package built on top of generator outputs.
- `tests` - xUnit test project (`tests/EtherSharp.Tests.csproj`).
- `bench` - BenchmarkDotNet project (`bench/EtherSharp.Bench.csproj`).
- `.editorconfig` - style, naming, and formatting source of truth.
- `.github/workflows/test-and-publish-to-nuget.yml` - CI sequence to mirror.

## Toolchain

- .NET SDK: `10.x`.
- Nullable: enabled.
- Implicit usings: enabled.
- Build includes analyzer/style enforcement, but `dotnet format` is required to enforce non-silent suggestion rules.

## Build / lint / test / pack

Run commands from repo root.

### Restore

- `dotnet restore`
- `dotnet restore --no-cache`

### Build

- `dotnet build`
- `dotnet build --configuration Release --no-restore`
- `dotnet build src/EtherSharp/EtherSharp.csproj`
- `dotnet build src/EtherSharp.Generator/EtherSharp.Generator.csproj`
- `dotnet build src/EtherSharp.ERC/EtherSharp.ERC.csproj`

### Lint / style enforcement

- Build diagnostics gate: `dotnet build --configuration Release --no-restore`
- Enforce all non-silent `.editorconfig` diagnostics:
- `dotnet format --verify-no-changes --severity info`
- Optional split checks:
- `dotnet format style --verify-no-changes --severity info`
- `dotnet format analyzers --verify-no-changes --severity info`
- `dotnet format whitespace --verify-no-changes`
- Reason: `--severity info` includes suggestion/warning/error and excludes silent.

### Tests

- `dotnet test`
- `dotnet test --configuration Release`
- `dotnet test tests/EtherSharp.Tests.csproj`

### Single test execution (important)

- One test method:
- `dotnet test tests/EtherSharp.Tests.csproj --filter "FullyQualifiedName~EtherSharp.Tests.ABI.Encoder.AddressAbiEncoderTests.Should_Match_Null_Address_Output"`
- One test class:
- `dotnet test tests/EtherSharp.Tests.csproj --filter "FullyQualifiedName~EtherSharp.Tests.ABI.Encoder.AddressAbiEncoderTests"`
- Method-name contains:
- `dotnet test tests/EtherSharp.Tests.csproj --filter "Name~Should_Match_Null_Address_Output"`
- Namespace segment:
- `dotnet test tests/EtherSharp.Tests.csproj --filter "FullyQualifiedName~EtherSharp.Tests.ABI.Decoder"`

### Coverage

- `dotnet test tests/EtherSharp.Tests.csproj --collect:"XPlat Code Coverage"`

### Pack / benchmark

- `dotnet pack --configuration Release --output ./nupkgs`
- `dotnet pack --configuration Release --no-build --output ./nupkgs`
- `dotnet pack src/EtherSharp.Generator/EtherSharp.Generator.csproj --configuration Release`
- `dotnet run --project bench/EtherSharp.Bench.csproj -c Release`

## CI sequence to mirror locally

- Build and pack `src/EtherSharp.Generator` first.
- Restore dependencies.
- Run full tests.
- Build Release with `--no-restore`.
- Pack artifacts to `./nupkgs`.

## Generated code policy

- `src/EtherSharp` uses `.tt` templates that emit checked-in `.cs` files.
- Source generator emits compile-time `*.generated.cs`.
- Fix source templates/writers, not generated outputs.

## Cursor / Copilot instruction files

- `.cursorrules`: not present.
- `.cursor/rules/`: not present.
- `.github/copilot-instructions.md`: not present.
- If these files appear later, treat them as higher-priority instructions.

## EditorConfig enforcement (all non-silent rules)

Silent exception (do not enforce):

- `csharp_prefer_static_local_function = false:silent`

Everything below is non-silent and enforceable.

### Global/editor settings

- `root = true`, `[*] end_of_line = lf`, `[*.{bat,cmd}] end_of_line = crlf`, `insert_final_newline = false`.
- `dotnet_hide_advanced_members = false`, `dotnet_member_insertion_location = with_other_members_of_the_same_kind`, `dotnet_property_generation_behavior = prefer_throwing_properties`, `dotnet_search_reference_assemblies = true`, `file_header_template = unset`.

### Imports and namespace placement

- `dotnet_separate_import_directive_groups = false`, `dotnet_sort_system_directives_first = false`, `csharp_using_directive_placement = outside_namespace:warning`.
- Use file-scoped namespaces: `csharp_style_namespace_declarations = file_scoped:error`.

### Qualification, predefined types, and var

- No `this.` qualification preference (warning): `dotnet_style_qualification_for_event/field/method/property = false:warning`.
- Prefer predefined keywords for locals/params/members: `dotnet_style_predefined_type_for_locals_parameters_members = true:warning`.
- Prefer framework type names for member access: `dotnet_style_predefined_type_for_member_access = false:warning`.
- Var rules: `csharp_style_var_elsewhere = true:suggestion`, `csharp_style_var_for_built_in_types = false:suggestion`, `csharp_style_var_when_type_is_apparent = true:suggestion`.

### Parentheses, operators, and general expressions

- Parentheses: arithmetic/other/relational binary operators `always_for_clarity:suggestion`; other operators `never_if_unnecessary:suggestion`.
- `dotnet_style_operator_placement_when_wrapping = beginning_of_line`, `csharp_space_around_binary_operators = before_and_after`.
- `dotnet_style_require_accessibility_modifiers = for_non_interface_members`.

### Dotnet expression/code-quality preferences

- Enabled: `dotnet_prefer_system_hash_code`, `dotnet_style_collection_initializer`, `dotnet_style_object_initializer`, `dotnet_style_explicit_tuple_names`, `dotnet_style_readonly_field`, `dotnet_style_prefer_is_null_check_over_reference_equality_method`, `dotnet_style_prefer_simplified_boolean_expressions`.
- Enabled: `dotnet_style_coalesce_expression`, `dotnet_style_null_propagation`, `dotnet_style_prefer_compound_assignment`, `dotnet_style_prefer_collection_expression`, `dotnet_style_prefer_inferred_anonymous_type_member_names`, `dotnet_style_prefer_inferred_tuple_names`, `dotnet_style_prefer_simplified_interpolation`, `dotnet_style_namespace_match_folder`.
- Conditionals/autoproperties/foreach cast: `dotnet_style_prefer_auto_properties = true:suggestion`, `dotnet_style_prefer_conditional_expression_over_assignment = true:suggestion`, `dotnet_style_prefer_conditional_expression_over_return = true:suggestion`, `dotnet_style_prefer_foreach_explicit_cast_in_source = when_strongly_typed`.
- Code-quality toggles: `dotnet_code_quality_unused_parameters = all`, `dotnet_remove_unnecessary_suppression_exclusions = none`.

### C# member/feature preferences

- Expression-bodied: accessors/indexers/lambdas/local functions/methods/operators `true` (mostly suggestion), properties `true:warning`, constructors `false:warning`.
- Pattern/null features: `csharp_style_pattern_matching_over_as_with_null_check = true:warning`, `csharp_style_pattern_matching_over_is_with_cast_check = true:warning`, `csharp_style_prefer_not_pattern = true:warning`, `csharp_style_prefer_pattern_matching = false`, `csharp_style_prefer_extended_property_pattern = true`, `csharp_style_conditional_delegate_call = true`, `csharp_style_prefer_null_check_over_type_check = true:warning`, `csharp_style_prefer_switch_expression = true`.
- Modifiers/features: `csharp_prefer_static_anonymous_function = true`, `csharp_preferred_modifier_order = public,private,protected,internal,file,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,required,volatile,async`, `csharp_style_prefer_readonly_struct = true`, `csharp_style_prefer_readonly_struct_member = true`, `csharp_prefer_simple_using_statement = true`, `csharp_prefer_system_threading_lock = true`, `csharp_style_prefer_method_group_conversion = true:suggestion`, `csharp_style_prefer_primary_constructors = true`, `csharp_style_prefer_top_level_statements = true:suggestion`.
- Additional expression rules: `csharp_prefer_simple_default_expression = true:warning`, `csharp_style_deconstructed_variable_declaration = true`, `csharp_style_implicit_object_creation_when_type_is_apparent = false:warning`, `csharp_style_inlined_variable_declaration = true`, `csharp_style_prefer_implicitly_typed_lambda_expression = true`, `csharp_style_prefer_index_operator = true`, `csharp_style_prefer_local_over_anonymous_function = true`, `csharp_style_prefer_range_operator = true`, `csharp_style_prefer_tuple_swap = true`, `csharp_style_prefer_unbound_generic_type_in_nameof = true`, `csharp_style_prefer_utf8_string_literals = true`, `csharp_style_throw_expression = true`, `csharp_style_unused_value_assignment_preference = discard_variable:warning`, `csharp_style_unused_value_expression_statement_preference = discard_variable`.

### Braces/newlines/blank lines

- Braces required: `csharp_prefer_braces = true:error`.
- Dotnet blank line rules: `dotnet_style_allow_multiple_blank_lines_experimental = false:warning`, `dotnet_style_allow_statement_immediately_after_block_experimental = true:suggestion`.
- C# blank line/embedded statement rules: `csharp_style_allow_blank_line_after_colon_in_constructor_initializer_experimental = true:warning`, `csharp_style_allow_blank_line_after_token_in_arrow_expression_clause_experimental = false:warning`, `csharp_style_allow_blank_line_after_token_in_conditional_expression_experimental = false:warning`, `csharp_style_allow_blank_lines_between_consecutive_braces_experimental = false:warning`, `csharp_style_allow_embedded_statements_on_same_line_experimental = false:warning`.
- Newline rules: `csharp_new_line_before_catch = true`, `csharp_new_line_before_else = true`, `csharp_new_line_before_finally = true`, `csharp_new_line_before_members_in_anonymous_types = true`, `csharp_new_line_before_members_in_object_initializers = true`, `csharp_new_line_before_open_brace = all`, `csharp_new_line_between_query_expression_clauses = true`.

### Indentation/spacing/preservation rules

- Indentation: `indent_size = 4`, `indent_style = space`, `tab_width = 4`, `csharp_indent_block_contents = true`, `csharp_indent_braces = false`, `csharp_indent_case_contents = true`, `csharp_indent_case_contents_when_block = false`, `csharp_indent_labels = one_less_than_current`, `csharp_indent_switch_labels = true`.
- Spacing: `csharp_space_after_cast = true`, `csharp_space_after_colon_in_inheritance_clause = true`, `csharp_space_after_comma = true`, `csharp_space_after_dot = false`, `csharp_space_after_keywords_in_control_flow_statements = false`, `csharp_space_after_semicolon_in_for_statement = true`, `csharp_space_around_declaration_statements = false`, `csharp_space_before_colon_in_inheritance_clause = true`, `csharp_space_before_comma = false`, `csharp_space_before_dot = false`, `csharp_space_before_open_square_brackets = false`, `csharp_space_before_semicolon_in_for_statement = false`, `csharp_space_between_empty_square_brackets = false`, `csharp_space_between_method_call_empty_parameter_list_parentheses = false`, `csharp_space_between_method_call_name_and_opening_parenthesis = false`, `csharp_space_between_method_call_parameter_list_parentheses = false`, `csharp_space_between_method_declaration_empty_parameter_list_parentheses = false`, `csharp_space_between_method_declaration_name_and_open_parenthesis = false`, `csharp_space_between_method_declaration_parameter_list_parentheses = false`, `csharp_space_between_parentheses = false`, `csharp_space_between_square_brackets = false`.
- Preserve toggles: `csharp_preserve_single_line_blocks = true`, `csharp_preserve_single_line_statements = false`.

### Naming rules and mappings

- Severity warning rules: constants all upper, interfaces begin with `I`, non-public fields begin with `_`, type parameters begin with `T`, types/methods/non-field members/public-protected fields PascalCase, parameters/locals camelCase.
- Naming symbol scopes are exactly as configured in `.editorconfig` (`interface`, `method`, `public_or_protected_field`, `private_or_protected_or_internal_field`, `types`, `non_field_members`, `type_parameters`, `parameter`, `local_variable`, `constants`).
- Naming style definitions are exactly as configured in `.editorconfig` (`pascal_case`, `begins_with_i`, `begins_with__`, `begins_with_t`, `camel_case`, `all_upper`).

## Coding expectations

- Validate inputs early; use precise exceptions with actionable messages.
- Prefer domain exceptions and `Try*` patterns for expected failure paths.
- Keep tests deterministic; use exact bytes/hex assertions for ABI encode/decode.
- Preserve span/`stackalloc`/low-allocation patterns in hot paths.

## Completion checklist for agents

- Apply smallest safe change.
- Run minimum necessary build/test; run full `dotnet test` for generator or ABI-surface changes.
- Run `dotnet format --verify-no-changes --severity info` before finalizing.
- Ensure no generated artifact was edited directly.
