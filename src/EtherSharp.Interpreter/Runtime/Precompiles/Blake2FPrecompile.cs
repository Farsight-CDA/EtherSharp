using EtherSharp.Interpreter.Crypto;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>The EIP-152 BLAKE2F precompile.</summary>
public sealed class Blake2FPrecompile : IPrecompile
{
    /// <summary>The shared instance.</summary>
    public static Blake2FPrecompile Instance { get; } = new();

    private Blake2FPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000009");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        if(!call.Gas.TryCharge(GetGasCost(call.Input.Span)))
        {
            return ValueTask.FromResult(ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.OutOfGas));
        }

        if(call.Input.Length != Blake2F.INPUT_LENGTH)
        {
            return ValueTask.FromResult(ExecutionResult.PrecompileFailure(PrecompileFailureReason.InvalidInputLength));
        }
        if(call.Input.Span[212] > 1)
        {
            return ValueTask.FromResult(ExecutionResult.PrecompileFailure(PrecompileFailureReason.Blake2FInvalidFinalBlockFlag));
        }

        byte[] output = new byte[Blake2F.OUTPUT_LENGTH];
        Blake2F.Compress(call.Input.Span, output);
        return ValueTask.FromResult(ExecutionResult.Success(output));
    }

    // Osaka gas price (EIP-152): one gas per round. Invalid lengths fail during execution.
    private static ulong GetGasCost(ReadOnlySpan<byte> input)
        => input.Length == Blake2F.INPUT_LENGTH
            ? BinaryPrimitives.ReadUInt32BigEndian(input)
            : 0;
}
