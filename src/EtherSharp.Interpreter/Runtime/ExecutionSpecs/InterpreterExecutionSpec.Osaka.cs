using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.ExecutionSpecs;

public sealed partial record InterpreterExecutionSpec
{
    /// <summary>
    /// Gets the Osaka code-size limits and standard precompile configuration.
    /// </summary>
    /// <remarks>
    /// Identity executes locally; other standard precompiles throw <see cref="NotSupportedException"/>.
    /// This preset does not imply full gas or consensus validation.
    /// </remarks>
    public static InterpreterExecutionSpec Osaka { get; } = new()
    {
        Precompiles = [
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000001")), // ECRECOVER
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000002")), // SHA-256
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000003")), // RIPEMD-160
            new IdentityPrecompile(), // 0x04: identity
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000005")), // MODEXP
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000006")), // BN254 ADD
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000007")), // BN254 MUL
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000008")), // BN254 PAIRING
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000009")), // BLAKE2F
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000a")), // KZG point evaluation
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000b")), // BLS12-381 G1ADD
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000c")), // BLS12-381 G1MSM
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000d")), // BLS12-381 G2ADD
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000e")), // BLS12-381 G2MSM
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000f")), // BLS12-381 PAIRING
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000010")), // BLS12-381 MAP_FP_TO_G1
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000011")), // BLS12-381 MAP_FP2_TO_G2
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000100")), // P256VERIFY (EIP-7951)
        ]
    };
}
