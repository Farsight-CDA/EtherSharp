# BLAKE2F compression

`Blake2F.tt` adapts the scalar compression implementation of
[Blake2Fast](https://github.com/saucecontrol/Blake2Fast), commit
`522ffc1770315635e7b2c32edb0feb593a7f7f2b`:

- `src/Blake2Fast/_BlakeScalar.ttinclude`
- `src/Blake2Fast/_BlakeAlg.ttinclude`

Copyright (c) 2018-2024 Clinton Ingram and Contributors. The original MIT notice
is retained in `Blake2Fast.LICENSE.txt` and included in the interpreter package.

EtherSharp changes:

- Expose the EIP-152 compression primitive with caller-supplied state, counters,
  final-block flag and a 32-bit round count, instead of whole-message hashing.
- Generate a repeating ten-round schedule with an exit after each round. Zero
  rounds still perform initialization and final state folding.
- Use endian-explicit decoding and portable scalar rotations and arithmetic.
- Use a single unrolled scalar implementation on every platform.
- Interleave each arithmetic stage across the four independent mixing lanes.

Regenerate from the repository root:

```sh
t4 src/EtherSharp.Interpreter/Crypto/Blake2F.tt
```
