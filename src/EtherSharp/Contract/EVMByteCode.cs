using EtherSharp.Contract.Sections;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Contract;

/// <summary>
/// Represents the source code of an EVM contract.
/// </summary>
public struct EVMByteCode
{
    /// <summary>
    /// The maximum runtime contract length.
    /// </summary>
    public const int MAX_RUNTIME_LENGTH = 24_576;
    /// <summary>
    /// The maximum init contract length.
    /// </summary>
    public const int MAX_INIT_LENGTH = 49_152;

    /// <summary>
    /// Creates an EVM bytecode wrapper from the provided raw bytes.
    /// </summary>
    /// <param name="byteCode">The runtime bytecode bytes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="byteCode"/> exceeds the maximum runtime length.</exception>
    public EVMByteCode(ReadOnlyMemory<byte> byteCode)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(byteCode.Length, MAX_RUNTIME_LENGTH, nameof(byteCode));
        ByteCode = byteCode;
    }

    /// <summary>
    /// Raw bytecode bytes.
    /// </summary>
    public ReadOnlyMemory<byte> ByteCode { get; }

    /// <summary>
    /// The length of the bytecode.
    /// </summary>
    public readonly int Length => ByteCode.Length;

    /// <summary>
    /// Checks if the given contract code implements the function from the given function section.
    /// </summary>
    /// <typeparam name="TFunctionsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasFunctions<TFunctionsSection>()
        where TFunctionsSection : IFunctionsSection
        => HasFunctions(TFunctionsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the events from the given logs section.
    /// </summary>
    /// <typeparam name="TLogsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasEvents<TLogsSection>()
        where TLogsSection : ILogsSection
        => HasEvents(TLogsSection.GetTopics());

    /// <summary>
    /// Checks if the given contract code implements the errors from the given errors section.
    /// </summary>
    /// <typeparam name="TErrorsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasErrors<TErrorsSection>()
        where TErrorsSection : IErrorsSection
        => HasErrors(TErrorsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the function from the given generated function type.
    /// </summary>
    /// <typeparam name="TFunction"></typeparam>
    /// <returns></returns>
    public readonly bool HasFunction<TFunction>()
        where TFunction : IGeneratedFunction
        => HasFunction(TFunction.SelectorBytes);

    /// <summary>
    /// Checks if the given contract code implements the event from the given generated log type.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <returns></returns>
    public readonly bool HasEvent<TLog>()
        where TLog : IGeneratedLog
        => HasEvent(TLog.TopicBytes);

    /// <summary>
    /// Checks if the given contract code implements the error from the given generated error type.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public readonly bool HasError<TError>()
        where TError : ISolidityError<TError>
        => HasError(TError.Selector);

    /// <summary>
    /// Checks if the given contract code implements a function selector.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public readonly bool HasFunction(ReadOnlyMemory<byte> selector)
    {
        Span<byte> prefixedSelector = stackalloc byte[5];
        prefixedSelector[0] = EvmOpcodeUtils.PUSH4;
        selector.Span.CopyTo(prefixedSelector[1..]);

        int index = ByteCode.Span.IndexOf(prefixedSelector);

        if(index == -1 || ByteCode.Length < (index + 7))
        {
            return false;
        }

        byte suffixByte = ByteCode.Span[index + 5];

        return EvmOpcodeUtils.ComparisonOpcodes.Contains(suffixByte);
    }

    /// <summary>
    /// Checks if the given contract code implements an event topic.
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public readonly bool HasEvent(ReadOnlyMemory<byte> topic)
    {
        Span<byte> prefixedSelector = stackalloc byte[33];
        prefixedSelector[0] = EvmOpcodeUtils.PUSH32;
        topic.Span.CopyTo(prefixedSelector[1..]);

        return ByteCode.Span.IndexOf(prefixedSelector) != -1;
    }

    /// <summary>
    /// Checks if the given contract code implements an error signature.
    /// </summary>
    /// <param name="signature"></param>
    /// <returns></returns>
    public readonly bool HasError(Bytes4 signature)
    {
        Span<byte> signatureBytes = stackalloc byte[4];
        signature.CopyTo(signatureBytes);
        Span<byte> shiftedSignatureBuffer = stackalloc byte[4];
        int shiftedSignatureLength = GetShiftedSignatureBytes(signatureBytes, shiftedSignatureBuffer);
        var shiftedSignature = shiftedSignatureBuffer[..shiftedSignatureLength];
        var byteCode = ByteCode.Span;

        return byteCode.IndexOf(signatureBytes) != -1
            || (!shiftedSignature.SequenceEqual(signatureBytes) && byteCode.IndexOf(shiftedSignature) != -1);
    }

    /// <summary>
    /// Checks if the given contract code implements a set of function selectors
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public readonly bool HasFunctions(params IEnumerable<ReadOnlyMemory<byte>> selectors)
    {
        foreach(var requiredSelector in selectors)
        {
            if(!HasFunction(requiredSelector))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the given contract code implements a set of event topics
    /// </summary>
    /// <param name="topics"></param>
    /// <returns></returns>
    public readonly bool HasEvents(params IEnumerable<ReadOnlyMemory<byte>> topics)
    {
        foreach(var requiredTopic in topics)
        {
            if(!HasEvent(requiredTopic))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the given contract code implements a set of error signatures.
    /// </summary>
    /// <param name="signatures"></param>
    /// <returns></returns>
    public readonly bool HasErrors(params IEnumerable<Bytes4> signatures)
    {
        foreach(var requiredSignature in signatures)
        {
            if(!HasError(requiredSignature))
            {
                return false;
            }
        }

        return true;
    }

    private static int GetShiftedSignatureBytes(ReadOnlySpan<byte> signature, Span<byte> shiftedSignature)
    {
        uint signatureValue = BinaryPrimitives.ReadUInt32BigEndian(signature);

        if(signatureValue == 0)
        {
            signature.CopyTo(shiftedSignature);
            return signature.Length;
        }

        int trailingZeroBits = BitOperations.TrailingZeroCount(signatureValue);
        if(trailingZeroBits == 0)
        {
            signature.CopyTo(shiftedSignature);
            return signature.Length;
        }

        signatureValue >>= trailingZeroBits;

        Span<byte> shiftedSignatureBytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(shiftedSignatureBytes, signatureValue);

        int leadingZeroBytes = BitOperations.LeadingZeroCount(signatureValue) / 8;
        shiftedSignatureBytes[leadingZeroBytes..].CopyTo(shiftedSignature);

        return 4 - leadingZeroBytes;
    }
}
