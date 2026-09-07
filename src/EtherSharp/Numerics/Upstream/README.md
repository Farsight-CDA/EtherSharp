# Vendored int256 core

Source: https://github.com/NethermindEth/int256

Pinned commit: `e9134ccdca3741ad48f04276bb532ee809e4b8d3`

`LICENSE` is copied from the upstream repository root and included in the
EtherSharp NuGet package.

## Local changes

There are two local source adaptations:

- Replace `public` with `internal` on top-level type declarations, including all partial declarations.
- In `UInt256.std.cs`, replace only the xxHash fallback in `GetHashCode()` with
  `HashCode.Combine` over eight `uint` limbs (low then high halves of `u0` through
  `u3`).

The original `Nethermind.Int256` namespace and source formatting are preserved.
The `.editorconfig` uses generated-code tooling exclusions to prevent formatting
and analyzer churn; these files are not generated.

## Manual updates

1. Check out the desired upstream revision in a temporary directory.
2. Replace the files listed above and `LICENSE` with copies from that revision.
   Review any upstream changes to the standard implementation's file list or dependencies.
3. Reapply the visibility and hashing adaptations documented above.
4. Compare against the pristine revision to confirm that only the documented
   adaptations differ, then review the update diff against the previous vendored version.
5. Update the pinned commit in this document and any required package dependency version.
6. Build EtherSharp and run the relevant repository checks.
