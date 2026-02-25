# EtherSharp Pre-1.0 Release Readiness Audit

This document captures issues that should be addressed before releasing `1.0.0`, when breaking changes become significantly more expensive.

For each item:
- **Context** explains what is happening now.
- **Impact** explains why this matters for a stable 1.0 contract.
- **Quick fix** gives a practical first remediation direction.

---

## P0 - Must Fix Before 1.0

## 1) HTTP cancellation is converted into transport errors

**References**
- `src/EtherSharp/RPC/Transport/HttpJsonRpcTransport.cs:141`
- `src/EtherSharp/RPC/Transport/HttpJsonRpcTransport.cs:182`
- `src/EtherSharp/RPC/RpcClient.cs:159`

**Context**
- `HttpJsonRpcTransport` catches broad `Exception` and wraps it as `RPCTransportException`, including cancellation exceptions.
- `RpcClient` expects cancellation to flow as `OperationCanceledException` (when token requested), so wrapping breaks cancellation semantics.

**Impact**
- Callers cannot reliably distinguish user cancellation from real transport faults.
- This becomes a public behavior contract once 1.0 ships.

**Quick fix**
- In HTTP transport, preserve `OperationCanceledException` when cancellation was requested.
- Narrow catch blocks and avoid wrapping cancellation exceptions.
- Add transport cancellation tests in `tests/RPC`.

---

## 2) Gas fee adjustment math is incorrect

**References**
- `src/EtherSharp/Tx/EIP1559/EIP1559GasFeeProvider.cs:112`
- `src/EtherSharp/Tx/EIP1559/EIP1559GasFeeProvider.cs:113`
- `src/EtherSharp/Tx/Legacy/LegacyGasFeeProvider.cs:62`
- `src/EtherSharp/Tx/Legacy/LegacyGasFeeProvider.cs:66`

**Context**
- Percentage scaling uses integer division in a way that truncates (`(100 + pct) / 100`), often collapsing to `1`.
- Legacy path then returns `gasPrice + adjustedGasPrice`, effectively double-counting in default scenarios.

**Impact**
- Transaction cost behavior can be economically wrong and inconsistent.
- Users may tune systems around incorrect behavior; fixing after 1.0 can become breaking.

**Quick fix**
- Use full integer math: `value * (100 + pct) / 100`.
- Return the adjusted legacy value once (not `gasPrice + adjustedGasPrice`).
- Add deterministic tests for config offsets and expected fee scaling.

---

## 3) Source generator constructor output can fail to compile

**References**
- `src/EtherSharp.Generator/SourceWriters/ContractFunctionSectionWriter.cs:120`

**Context**
- Generated constructor code currently emits `encoder.TryWritoTo(...)` (typo), which does not match runtime API naming.

**Impact**
- ABIs with constructor inputs can produce uncompilable generated code.
- This blocks core generator workflow and undermines package trust for 1.0.

**Quick fix**
- Correct to `TryWriteTo` in generator writer.
- Add generator regression test using constructor args to ensure generated code compiles.

---

## 4) Generator interface discovery is too narrow and can silently skip valid contracts

**References**
- `src/EtherSharp.Generator/Generator.cs:45`
- `src/EtherSharp.Generator/Generator.cs:49`

**Context**
- Syntax prefilter only matches direct `IdentifierNameSyntax` text `IEVMContract`.
- Qualified names or alternate interface declaration shapes can be skipped before semantic checks.

**Impact**
- Expected generation may not happen, often with poor diagnostics.
- Silent misses are especially costly when API behavior is being locked for 1.0.

**Quick fix**
- Broaden syntax candidate collection (all partial interfaces with base list), then use semantic checks for true eligibility.
- Emit explicit diagnostics when a candidate looks intended but unsupported.

---

## 5) WebSocket subscription handling can fail whole processing path on malformed payloads

**References**
- `src/EtherSharp/RPC/Transport/WssJsonRpcTransport.cs:228`
- `src/EtherSharp/Client/Services/Subscriptions/SubscriptionsManager.cs:106`
- `src/EtherSharp/Realtime/Events/Subscription/EventSubscription.cs:50`
- `src/EtherSharp/Realtime/Blocks/Subscription/BlocksSubscription.cs:42`

**Context**
- Subscription payload deserialization/handler failures are not isolated.
- Exceptions can bubble through message processing and destabilize reconnect/flow behavior.

**Impact**
- Node/provider payload quirks can disrupt otherwise healthy subscriptions.
- Reliability expectations become hard to adjust after 1.0.

**Quick fix**
- Wrap subscription dispatch in protective try/catch at manager/transport boundaries.
- Isolate per-subscription failures and log/drop malformed payloads instead of impacting global loop.
- Add malformed-payload resilience tests.

---

## 6) Unbounded memory pressure risks from untrusted payloads

**References**
- `src/EtherSharp/RPC/Transport/WssJsonRpcTransport.cs:170`
- `src/EtherSharp/Realtime/Events/Subscription/EventSubscription.cs:28`
- `src/EtherSharp/Realtime/Blocks/Subscription/BlocksSubscription.cs:18`
- `src/EtherSharp/ABI/Types/AbiTypes.Array.cs:48`

**Context**
- WebSocket message assembly uses unbounded `MemoryStream` growth.
- Subscription channels are unbounded.
- ABI array decoding allocates arrays directly from decoded lengths without defensive caps.

**Impact**
- Remote endpoints can trigger memory exhaustion patterns.
- Introducing hard limits later can be behaviorally breaking once users depend on current behavior.

**Quick fix**
- Add configurable limits (message bytes, channel capacity, max decode collection sizes).
- Fail fast with explicit exceptions and clear configuration docs.
- Add adversarial tests for oversized payloads.

---

## 7) Release workflows publish on every `master` push

**References**
- `.github/workflows/test-and-publish-to-nuget.yml:4`
- `.github/workflows/test-and-publish-to-private-feed.yml:4`

**Context**
- Current workflows combine CI and publishing and trigger publication on normal branch pushes.

**Impact**
- High risk of accidental/undesired releases.
- Weakens release governance expected for 1.0 lifecycle.

**Quick fix**
- Split CI and publish workflows.
- Publish only on version tags (`v*`) or manual release workflow with approvals.
- Keep CI (tests/format/compat checks) on PR + push.

---

## P1 - Strongly Recommended Before 1.0

## 8) Pending tx confirmation API has no cancellation token

**References**
- `src/EtherSharp/Tx/PendingHandler/IPendingTxHandler.cs:30`
- `src/EtherSharp/Client/Services/TxScheduler/BlockingSequentialTxScheduler.cs:355`
- `src/EtherSharp/Tx/PendingHandler/PendingTxHandler.cs:76`

**Context**
- `PublishAndConfirmAsync` exposes no cancellation token.
- Internal waits/timeouts are mostly scheduler-owned; handler also converts exceptions into result objects.

**Impact**
- Long-running transaction flows are difficult to cancel predictably.
- Cancellation semantics become harder to introduce cleanly after 1.0.

**Quick fix**
- Add cancellable overloads now and propagate token through publish/delay/confirmation paths.
- Define and document cancellation result/exception semantics.

---

## 9) Revert parsing is brittle across providers

**References**
- `src/EtherSharp/Types/TxCallResult.cs:52`
- `src/EtherSharp/Types/TxCallResult.cs:62`

**Context**
- Revert detection depends on `Message.StartsWith("execution reverted")` and assumes `error.Data` hex string shape.

**Impact**
- Provider differences can misclassify errors or fail parsing.
- Exception taxonomy can become inconsistent for users.

**Quick fix**
- Parse using richer RPC error signals (code + robust data extraction).
- Support alternate data shapes and case-insensitive/provider-variant messages.
- Add tests with multiple provider payload formats.

---

## 10) Contract factory concurrency and name-collision risks

**References**
- `src/EtherSharp/Client/Services/ContractFactory/ContractFactory.cs:17`
- `src/EtherSharp/Client/Services/ContractFactory/ContractFactory.cs:49`

**Context**
- Reads from `Dictionary` happen outside lock while writes are locked.
- Generated implementation lookup uses simple type name suffix matching only.

**Impact**
- Potential race conditions under concurrent usage.
- Type-name collisions across namespaces can fail or resolve ambiguously.

**Quick fix**
- Move to `ConcurrentDictionary` + `GetOrAdd` (or lock all access consistently).
- Resolve implementation types by fully qualified identity.

---

## 11) Public internal-surface API may lock in undesired maintenance burden

**References**
- `src/EtherSharp/Client/IInternalEtherClient.cs:8`
- `src/EtherSharp/Client/IEtherClient.cs:123`

**Context**
- Internal service-provider and raw RPC access are exposed publicly via `AsInternal()`.

**Impact**
- Consumers may build strong coupling to internals, making future refactors breaking.

**Quick fix**
- Decide explicit support policy now: either keep/document as supported extensibility or reduce visibility/scope before 1.0.

---

## 12) ERC4626 namespace mismatch should be corrected before freeze

**References**
- `src/EtherSharp.ERC/ERC4626/IERC4626.cs:3`

**Context**
- `IERC4626` is currently declared in `EtherSharp.ERC.ERC721` namespace.

**Impact**
- Public API shape is semantically wrong and confusing.
- Post-1.0 correction would be a namespace breaking change.

**Quick fix**
- Move to `EtherSharp.ERC.ERC4626` now.
- If needed, provide temporary bridge/obsolete forwarders pre-1.0.

---

## 13) Generator diagnostics contain a mismapped descriptor path

**References**
- `src/EtherSharp.Generator/Generator.cs:198`
- `src/EtherSharp.Generator/GeneratorDiagnostics.cs:94`

**Context**
- Duplicate bytecode filename branch reports `MultipleBytecodeFileAttributeFound` instead of `MultipleBytecodeFilesWithNameFound`.

**Impact**
- Error messaging is less precise, increasing troubleshooting cost for users.

**Quick fix**
- Wire the correct descriptor for duplicate-filename cases and add test coverage for this diagnostic.

---

## P2 - Hardening and Process Improvements

## 14) No API compatibility gate for semver protection

**References**
- `.github/workflows/test-and-publish-to-nuget.yml`
- `src/EtherSharp/EtherSharp.csproj`
- `src/EtherSharp.ERC/EtherSharp.ERC.csproj`

**Context**
- No automated API baseline/package compatibility check appears in CI.

**Impact**
- Accidental breaking API changes can slip in unnoticed.

**Quick fix**
- Add API compat tooling (e.g., package validation/ApiCompat) and fail CI for unapproved breaks.

---

## 15) CI coverage and PR gating are incomplete

**References**
- `.github/workflows/test-and-publish-to-nuget.yml:3`
- `.github/workflows/test-and-publish-to-private-feed.yml:3`
- `tests/EtherSharp.Tests.csproj:16`

**Context**
- Workflows do not currently run on PR.
- Coverage tooling exists (`coverlet.collector`) but no threshold enforcement.

**Impact**
- Reduced confidence during stabilization phase.

**Quick fix**
- Add PR triggers and coverage publish/threshold checks.
- Keep format gate in CI (`dotnet format --verify-no-changes --severity info`).

---

## 16) Exception taxonomy still includes multiple `NotImplementedException` fallbacks

**References**
- `src/EtherSharp/RPC/Modules/Eth/EthRpcModule.cs:20`
- `src/EtherSharp/Types/TxCallResult.cs:33`
- `src/EtherSharp/RPC/Modules/Debug/DebugRpcModule.cs:17`

**Context**
- Operational paths use `NotImplementedException` as fallback branches.

**Impact**
- Public failure behavior is less predictable/expressive.

**Quick fix**
- Replace with explicit domain exceptions (`RPCException`, `RPCTransportException`, or invariant-focused `InvalidOperationException`).

---

## 17) Test coverage is broad in ABI surface but sparse in critical runtime paths

**References**
- `tests/`
- `src/EtherSharp/Tx/`
- `src/EtherSharp/Client/Services/`
- `src/EtherSharp/RPC/Transport/`
- `src/EtherSharp.Generator/`

**Context**
- Existing suite has extensive ABI tests, but fewer stress tests around tx lifecycle, cancellation, reconnect behavior, malformed payload handling, and generator edge-case matrices.

**Impact**
- Runtime regressions are more likely in production-critical paths.

**Quick fix**
- Add targeted suites for tx scheduler, transport cancellation/reconnect, malformed RPC payloads, and generator compilation golden tests.

---

## Validation Run During Audit

- `dotnet test EtherSharp.slnx --configuration Release` passed (`1181` tests).
- `dotnet format --verify-no-changes --severity info` passed.
