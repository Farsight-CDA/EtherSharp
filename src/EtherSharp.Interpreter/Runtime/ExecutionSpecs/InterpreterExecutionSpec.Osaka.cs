using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Types;
using System.Collections.Immutable;

namespace EtherSharp.Interpreter.Runtime.ExecutionSpecs;

public sealed partial record InterpreterExecutionSpec
{
    /// <summary>
    /// Gets the Osaka code-size limits and standard precompile configuration with Osaka gas pricing.
    /// </summary>
    /// <remarks>
    /// ECRECOVER, SHA-256, RIPEMD-160, identity, ModExp, BLAKE2F, and P256VERIFY execute locally;
    /// other standard precompiles throw <see cref="NotSupportedException"/>.
    /// This preset does not imply full gas or consensus validation.
    /// </remarks>
    public static InterpreterExecutionSpec Osaka { get; } = new()
    {
        FixedOpcodeGasCosts = [.. Enumerable.Range(0, 256)
            .Select<int, ulong>(opcode => (EvmOpcode) opcode switch
            {
                EvmOpcode.Add or EvmOpcode.Sub => 3,
                EvmOpcode.Mul or EvmOpcode.Div or EvmOpcode.SDiv
                    or EvmOpcode.Mod or EvmOpcode.SMod or EvmOpcode.SignExtend => 5,
                EvmOpcode.AddMod or EvmOpcode.MulMod => 8,
                EvmOpcode.Exp => 10,
                >= EvmOpcode.Lt and <= EvmOpcode.Sar => 3,
                EvmOpcode.Clz => 5,
                EvmOpcode.Keccak256 => 30,

                EvmOpcode.Address or EvmOpcode.Origin or EvmOpcode.Caller or EvmOpcode.CallValue
                    or EvmOpcode.CallDataSize or EvmOpcode.CodeSize or EvmOpcode.GasPrice
                    or EvmOpcode.ReturnDataSize or EvmOpcode.Coinbase or EvmOpcode.Timestamp
                    or EvmOpcode.Number or EvmOpcode.PrevRandao or EvmOpcode.GasLimit
                    or EvmOpcode.ChainId or EvmOpcode.BaseFee or EvmOpcode.BlobBaseFee => 2,
                EvmOpcode.CallDataLoad or EvmOpcode.CallDataCopy or EvmOpcode.CodeCopy
                    or EvmOpcode.ReturnDataCopy or EvmOpcode.BlobHash => 3,
                EvmOpcode.BlockHash => 20,
                EvmOpcode.SelfBalance => 5,

                // Warm access is the fixed baseline. Cold-access premiums are dynamic.
                EvmOpcode.Balance or EvmOpcode.ExtCodeSize or EvmOpcode.ExtCodeCopy
                    or EvmOpcode.ExtCodeHash or EvmOpcode.SLoad
                    or EvmOpcode.Call or EvmOpcode.CallCode or EvmOpcode.DelegateCall
                    or EvmOpcode.StaticCall => 100,

                EvmOpcode.Pop or EvmOpcode.Pc or EvmOpcode.MSize or EvmOpcode.Gas
                    or EvmOpcode.Push0 => 2,
                EvmOpcode.MLoad or EvmOpcode.MStore or EvmOpcode.MStore8 or EvmOpcode.MCopy => 3,
                EvmOpcode.Jump => 8,
                EvmOpcode.JumpI => 10,
                EvmOpcode.JumpDest => 1,
                EvmOpcode.TLoad or EvmOpcode.TStore => 100,
                >= EvmOpcode.Push1 and <= EvmOpcode.Swap16 => 3,
                >= EvmOpcode.Log0 and <= EvmOpcode.Log4 => 375,
                EvmOpcode.Create or EvmOpcode.Create2 => 32000,
                EvmOpcode.SelfDestruct => 5000,

                // SSTORE is priced entirely by its dynamic rules, including the stipend check.
                EvmOpcode.SStore or EvmOpcode.Stop or EvmOpcode.Return or EvmOpcode.Revert => 0,
                // Invalid/undefined bytes still halt exceptionally in the opcode handler.
                _ => 0
            })
        ],
        GasParameters = new()
        {
            Memory = new()
            {
                LinearCostPerWord = 3,
                QuadraticDivisor = 512
            }
        },
        Precompiles = [
            EcRecoverPrecompile.Instance, // 0x01: ECRECOVER
            Sha256Precompile.Instance, // 0x02: SHA-256
            Ripemd160Precompile.Instance, // 0x03: RIPEMD-160
            IdentityPrecompile.Instance, // 0x04: identity
            new ModExpPrecompile(maxOperandLength: 1024), // 0x05: MODEXP (EIP-7823)
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000006")), // BN254 ADD
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000007")), // BN254 MUL
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000008")), // BN254 PAIRING
            Blake2FPrecompile.Instance, // 0x09: BLAKE2F
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000a")), // KZG point evaluation
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000b")), // BLS12-381 G1ADD
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000c")), // BLS12-381 G1MSM
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000d")), // BLS12-381 G2ADD
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000e")), // BLS12-381 G2MSM
            new UnsupportedPrecompile(Address.FromString("0x000000000000000000000000000000000000000f")), // BLS12-381 PAIRING
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000010")), // BLS12-381 MAP_FP_TO_G1
            new UnsupportedPrecompile(Address.FromString("0x0000000000000000000000000000000000000011")), // BLS12-381 MAP_FP2_TO_G2
            P256VerifyPrecompile.Instance, // 0x0100: P256VERIFY (EIP-7951)
        ]
    };
}
