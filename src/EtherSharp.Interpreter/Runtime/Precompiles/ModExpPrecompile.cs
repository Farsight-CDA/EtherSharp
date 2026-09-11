using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EVM modular exponentiation precompile with a configured operand-length limit.
/// </summary>
public sealed class ModExpPrecompile : IPrecompile
{
    private const int HEADER_LENGTH = 96;
    private const int WORD_LENGTH = 32;

    private readonly UInt256 _maxOperandLength;

    /// <summary>The per-operand size limit in bytes.</summary>
    public int MaxOperandLength { get; }

    /// <summary>Creates a precompile with the given operand-size limit.</summary>
    /// <param name="maxOperandLength">The maximum permitted length of each operand in bytes.</param>
    /// <exception cref="ArgumentOutOfRangeException">The operand-length limit is negative.</exception>
    public ModExpPrecompile(int maxOperandLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxOperandLength);
        MaxOperandLength = maxOperandLength;
        _maxOperandLength = (UInt256) maxOperandLength;
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000005");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => ValueTask.FromResult(GetGasCost(call.Input.Span) is { } cost && call.Gas.TryCharge(cost)
            ? Execute(call.Input.Span)
            : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.OutOfGas)
        );

    // Osaka gas prices (EIP-7883). Null means the cost exceeds any ulong gas budget.
    private static ulong? GetGasCost(ReadOnlySpan<byte> input)
    {
        var baseLength = ReadPaddedWord(input, 0, WORD_LENGTH);
        var exponentLength = ReadPaddedWord(input, WORD_LENGTH, WORD_LENGTH);
        var modulusLength = ReadPaddedWord(input, 2 * WORD_LENGTH, WORD_LENGTH);
        if(baseLength > (UInt256) UInt64.MaxValue
            || exponentLength > (UInt256) UInt64.MaxValue
            || modulusLength > (UInt256) UInt64.MaxValue)
        {
            return null;
        }

        ulong baseSize = (ulong) baseLength;
        ulong exponentSize = (ulong) exponentLength;
        ulong maxSize = Math.Max(baseSize, (ulong) modulusLength);
        var exponentHead = input.Length >= HEADER_LENGTH && baseSize <= (ulong) (input.Length - HEADER_LENGTH)
            ? ReadPaddedWord(input, HEADER_LENGTH + (int) baseSize, (int) Math.Min(exponentSize, WORD_LENGTH))
            : UInt256.Zero;

        // The leading (up to) 32 exponent bytes include trailing zero padding.
        var iterations = exponentSize > WORD_LENGTH ? (UInt128) (exponentSize - WORD_LENGTH) * 16 : 0;
        if(!exponentHead.IsZero)
        {
            iterations += (uint) (255 - UInt256.LeadingZeroCount(in exponentHead));
        }
        iterations = UInt128.Max(iterations, 1);
        var words = ((UInt128) maxSize + 7) / 8;
        var complexity = maxSize <= WORD_LENGTH ? 16 : 2 * words * words;

        // Avoid narrowing or multiplying an unaffordable cost, even for full-width declared lengths.
        return complexity > UInt64.MaxValue / iterations
            ? null
            : Math.Max(500UL, (ulong) (complexity * iterations));
    }

    private static UInt256 ReadPaddedWord(ReadOnlySpan<byte> input, int offset, int length)
    {
        Span<byte> word = stackalloc byte[WORD_LENGTH];
        word.Clear();
        if(offset < input.Length)
        {
            input.Slice(offset, Math.Min(length, input.Length - offset)).CopyTo(word[(WORD_LENGTH - length)..]);
        }
        return BinaryPrimitives.ReadUInt256BigEndian(word);
    }

    private ExecutionResult Execute(ReadOnlySpan<byte> input)
    {
        // EIP-198 treats all missing input bytes, including header bytes, as trailing zeros.
        Span<byte> header = stackalloc byte[HEADER_LENGTH];
        header.Clear();
        input[..Math.Min(input.Length, HEADER_LENGTH)].CopyTo(header);

        var baseLength = BinaryPrimitives.ReadUInt256BigEndian(header);
        var exponentLength = BinaryPrimitives.ReadUInt256BigEndian(header[WORD_LENGTH..]);
        var modulusLength = BinaryPrimitives.ReadUInt256BigEndian(header[(2 * WORD_LENGTH)..]);

        // Validate full-width lengths before narrowing or taking any empty-result shortcut.
        if(baseLength > _maxOperandLength || exponentLength > _maxOperandLength || modulusLength > _maxOperandLength)
        {
            return ExecutionResult.PrecompileFailure(PrecompileFailureReason.ModExpOperandLengthExceeded);
        }
        if(modulusLength.IsZero)
        {
            return ExecutionResult.Success();
        }

        int baseSize = (int) baseLength;
        int exponentSize = (int) exponentLength;
        int modulusSize = (int) modulusLength;
        Span<byte> operands = stackalloc byte[checked(baseSize + exponentSize + modulusSize)];
        operands.Clear();
        if(input.Length > HEADER_LENGTH)
        {
            input.Slice(HEADER_LENGTH, Math.Min(input.Length - HEADER_LENGTH, operands.Length)).CopyTo(operands);
        }

        // Declared lengths determine offsets and output width; significant bytes select the numeric path.
        var baseBytes = operands[..baseSize].TrimStart((byte) 0);
        var exponentBytes = operands.Slice(baseSize, exponentSize).TrimStart((byte) 0);
        var modulusBytes = operands[(baseSize + exponentSize)..].TrimStart((byte) 0);
        byte[] output = new byte[modulusSize];
        if(modulusBytes.IsEmpty)
        {
            return ExecutionResult.Success(output);
        }

        if(baseBytes.Length <= WORD_LENGTH && exponentBytes.Length <= WORD_LENGTH && modulusBytes.Length <= WORD_LENGTH)
        {
            var baseValue = ReadUInt256(baseBytes);
            var exponent = ReadUInt256(exponentBytes);
            var modulus = ReadUInt256(modulusBytes);
            UInt256.ExpMod(baseValue, exponent, modulus, out var result);

            Span<byte> word = stackalloc byte[WORD_LENGTH];
            BinaryPrimitives.WriteUInt256BigEndian(word, result);
            int resultSize = Math.Min(WORD_LENGTH, modulusSize);
            word[(WORD_LENGTH - resultSize)..].CopyTo(output.AsSpan(modulusSize - resultSize));
        }
        else
        {
            var baseValue = new BigInteger(baseBytes, isUnsigned: true, isBigEndian: true);
            var exponent = new BigInteger(exponentBytes, isUnsigned: true, isBigEndian: true);
            var modulus = new BigInteger(modulusBytes, isUnsigned: true, isBigEndian: true);
            var result = BigInteger.ModPow(baseValue, exponent, modulus);
            _ = result.TryWriteBytes(output.AsSpan(modulusSize - result.GetByteCount(isUnsigned: true)), out _, isUnsigned: true, isBigEndian: true);
        }

        return ExecutionResult.Success(output);
    }

    private static UInt256 ReadUInt256(ReadOnlySpan<byte> bytes)
    {
        Span<byte> word = stackalloc byte[WORD_LENGTH];
        word.Clear();
        bytes.CopyTo(word[(WORD_LENGTH - bytes.Length)..]);
        return BinaryPrimitives.ReadUInt256BigEndian(word);
    }
}
