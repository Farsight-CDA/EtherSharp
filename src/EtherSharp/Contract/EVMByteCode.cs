using EtherSharp.Contract.Sections;
using EtherSharp.Types;

namespace EtherSharp.Contract;

/// <summary>
/// Represents the source code of an EVM contract.
/// </summary>
/// <param name="byteCode">The bytecode bytes.</param>
public struct EVMByteCode(ReadOnlyMemory<byte> byteCode)
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
    /// Raw bytecode bytes.
    /// </summary>
    public ReadOnlyMemory<byte> ByteCode { get; } = byteCode;

    /// <summary>
    /// The length of the bytecode.
    /// </summary>
    public readonly int Length => ByteCode.Length;

    /// <summary>
    /// Checks if the given opcode exists in the contract code, ignoring bytes used as PUSH data and compiler metadata.
    /// </summary>
    /// <param name="opcode">The opcode byte to find.</param>
    /// <returns>True if the opcode appears outside PUSH data; otherwise false.</returns>
    public readonly bool ContainsOpcode(byte opcode)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;

        for(int i = 0; i < byteCode.Length; i++)
        {
            byte currentOpcode = byteCode[i];

            if(currentOpcode == opcode)
            {
                return true;
            }

            if(EvmOpcodeUtils.TryGetPushLength(currentOpcode, out int pushLength))
            {
                i += pushLength;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the given contract code implements the function from the given function section.
    /// </summary>
    /// <typeparam name="TFunctionsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasFunctions<TFunctionsSection>()
        where TFunctionsSection : IFunctionsSection
        => HasFunctions(TFunctionsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the function from the given generated function type.
    /// </summary>
    /// <typeparam name="TFunction"></typeparam>
    /// <returns></returns>
    public readonly bool HasFunction<TFunction>()
        where TFunction : IGeneratedFunction
        => HasFunction(TFunction.SelectorBytes);

    /// <summary>
    /// Checks if the given contract code implements a set of function selectors
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public readonly bool HasFunctions(IEnumerable<ReadOnlyMemory<byte>> selectors)
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
    /// Checks if the given contract code implements a function selector.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public readonly bool HasFunction(ReadOnlyMemory<byte> selector)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;
        var normalizedSelector = selector.Span.TrimStart((byte) 0);
        normalizedSelector = normalizedSelector.IsEmpty ? selector.Span[^1..] : normalizedSelector;

        for(int i = 0; i < byteCode.Length; i++)
        {
            if(!EvmOpcodeUtils.TryGetPushLength(byteCode[i], out int pushLength))
            {
                continue;
            }

            int pushValueIndex = i + 1;
            int pushEndIndex = pushValueIndex + pushLength;

            if(pushEndIndex > byteCode.Length)
            {
                break;
            }

            if(pushLength == normalizedSelector.Length
                && byteCode[pushValueIndex..pushEndIndex].SequenceEqual(normalizedSelector)
                && pushEndIndex < byteCode.Length
                && EvmOpcodeUtils.ComparisonOpcodes.Contains(byteCode[pushEndIndex]))
            {
                return true;
            }

            i += pushLength;
        }

        return false;
    }

    /// <summary>
    /// Checks if the given contract code implements the events from the given logs section.
    /// </summary>
    /// <typeparam name="TLogsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasEvents<TLogsSection>()
        where TLogsSection : ILogsSection
        => HasEvents(TLogsSection.GetTopics());

    /// <summary>
    /// Checks if the given contract code implements the event from the given generated log type.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <returns></returns>
    public readonly bool HasEvent<TLog>()
        where TLog : IGeneratedLog
        => HasEvent(TLog.TopicBytes);

    /// <summary>
    /// Checks if the given contract code implements a set of event topics
    /// </summary>
    /// <param name="topics"></param>
    /// <returns></returns>
    public readonly bool HasEvents(IEnumerable<ReadOnlyMemory<byte>> topics)
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
    /// Checks if the given contract code implements an event topic.
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public readonly bool HasEvent(ReadOnlyMemory<byte> topic)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;

        if(byteCode.IndexOf(topic.Span) != -1)
        {
            return true;
        }

        var normalizedTopic = topic.Span.TrimStart((byte) 0);
        normalizedTopic = normalizedTopic.IsEmpty ? topic.Span[^1..] : normalizedTopic;

        return normalizedTopic.Length != topic.Length
            && byteCode.IndexOf(normalizedTopic) != -1;
    }

    /// <summary>
    /// Checks if the given contract code implements the errors from the given errors section.
    /// </summary>
    /// <typeparam name="TErrorsSection"></typeparam>
    /// <returns></returns>
    public readonly bool HasErrors<TErrorsSection>()
        where TErrorsSection : IErrorsSection
        => HasErrors(TErrorsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the error from the given generated error type.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public readonly bool HasError<TError>()
        where TError : ISolidityError<TError>
        => HasError(TError.Selector);

    /// <summary>
    /// Checks if the given contract code implements a set of error signatures.
    /// </summary>
    /// <param name="signatures"></param>
    /// <returns></returns>
    public readonly bool HasErrors(IEnumerable<Bytes4> signatures)
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

    /// <summary>
    /// Checks if the given contract code implements an error signature.
    /// </summary>
    /// <param name="signature"></param>
    /// <returns></returns>
    public readonly bool HasError(Bytes4 signature)
    {
        Span<byte> signatureBytes = stackalloc byte[4];
        signature.CopyTo(signatureBytes);
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;

        return HasPushedErrorSignature(byteCode, signatureBytes);
    }

    private static bool HasPushedErrorSignature(ReadOnlySpan<byte> byteCode, ReadOnlySpan<byte> signature)
    {
        Span<byte> compactSignature = stackalloc byte[4];
        int compactSignatureLength = CopyWithoutLeadingOrTrailingZeroBytes(signature, compactSignature);

        for(int i = 0; i < byteCode.Length; i++)
        {
            if(!EvmOpcodeUtils.TryGetPushLength(byteCode[i], out int pushLength))
            {
                continue;
            }

            int pushValueIndex = i + 1;
            int pushEndIndex = pushValueIndex + pushLength;

            if(pushEndIndex > byteCode.Length)
            {
                break;
            }

            var pushData = byteCode[pushValueIndex..pushEndIndex];

            if(pushData.IndexOf(signature) != -1
                || (compactSignatureLength != signature.Length && pushData.IndexOf(compactSignature[..compactSignatureLength]) != -1))
            {
                return true;
            }

            i += pushLength;
        }

        return false;
    }

    private static int CopyWithoutLeadingOrTrailingZeroBytes(ReadOnlySpan<byte> bytes, Span<byte> destination)
    {
        int leadingZeroBytes = 0;

        while(leadingZeroBytes < bytes.Length - 1 && bytes[leadingZeroBytes] == 0)
        {
            leadingZeroBytes++;
        }

        int trailingZeroBytes = 0;

        while(trailingZeroBytes < bytes.Length - leadingZeroBytes - 1 && bytes[bytes.Length - trailingZeroBytes - 1] == 0)
        {
            trailingZeroBytes++;
        }

        bytes[leadingZeroBytes..(bytes.Length - trailingZeroBytes)].CopyTo(destination);

        return bytes.Length - leadingZeroBytes - trailingZeroBytes;
    }
}
