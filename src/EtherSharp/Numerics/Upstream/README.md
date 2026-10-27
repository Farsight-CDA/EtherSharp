# Vendored int256 core

Source: https://github.com/NethermindEth/int256

Pinned commit: `fab76ab4d63d5f4cf2d04fb49876c5f6158b1e56` (upstream `main`, version 1.12.0)

The vendored sources are all `.cs` files in upstream's `src/Nethermind.Int256`,
except `UInt256.zkevm.cs`. EtherSharp uses the standard implementation in
`UInt256.std.cs`.
The existing `UInt256.Conversions.cs`, `UInt256.Ctors.cs`, `UInt256.Lookups.cs`,
and `UInt256.Operators.cs` filenames retain their capitalization; upstream uses
lowercase partial names.

`LICENSE` and `THIRD-PARTY-NOTICES` are copied from the upstream repository root.
Both files are included in the EtherSharp NuGet package.

## Local changes

There are three local source adaptations:

- Replace `public` with `internal` on top-level type declarations, including all partial declarations.
- In `UInt256.std.cs`, replace only the xxHash fallback in `GetHashCode()` with
  `HashCode.Combine` over eight `uint` limbs (low then high halves of `u0` through
  `u3`).
- In `UInt256.divmod.cs`, change `Multiply256To512Bit` from `private` to
  `internal` for the existing full-width multiplication wrappers.

The original `Nethermind.Int256` namespace and source formatting are preserved.
The `.editorconfig` uses generated-code tooling exclusions to prevent formatting
and analyzer churn; these files are not generated.

## Manual updates

1. Check out the desired upstream revision in a temporary directory.
2. Apply the upstream Git diff from the pinned revision to the desired revision
   for the source file set described above, `LICENSE`, and `THIRD-PARTY-NOTICES`.
   Review any upstream changes to the standard implementation's file list or dependencies.
3. Preserve or reapply the visibility, hashing, and multiplication-helper adaptations documented above.
4. Compare against the pristine revision to confirm that only the documented
   adaptations differ, then review the update diff against the previous vendored version.
5. Update the pinned commit in this document and any required package dependency version.
6. Build EtherSharp and run the relevant repository checks.
