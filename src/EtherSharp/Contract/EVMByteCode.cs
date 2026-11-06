using EtherSharp.Contract.Sections;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Contract;

/// <summary>
/// Represents the source code of an EVM contract.
/// </summary>
/// <remarks>Instances can be shared across calls and interpreters, including their lazily initialized analysis.</remarks>
/// <param name="byteCode">The bytecode bytes. The backing memory must not be modified while the bytecode is in use.</param>
public sealed class EVMByteCode(ReadOnlyMemory<byte> byteCode) : IEquatable<EVMByteCode>
{
    private readonly Lazy<PushDataMap> _pushDataMap = new(() => new(byteCode.Span));
    internal PushDataMap PushDataMap => _pushDataMap.Value;

    /// <summary>
    /// The maximum runtime contract length.
    /// </summary>
    public const int MAX_RUNTIME_LENGTH = 24_576;
    /// <summary>
    /// The maximum init contract length.
    /// </summary>
    public const int MAX_INIT_LENGTH = 49_152;

    /// <summary>
    /// Gets empty EVM bytecode.
    /// </summary>
    public static EVMByteCode Empty { get; } = new(ReadOnlyMemory<byte>.Empty);

    /// <summary>
    /// Raw bytecode bytes.
    /// </summary>
    public ReadOnlyMemory<byte> ByteCode { get; } = byteCode;

    /// <summary>
    /// The length of the bytecode.
    /// </summary>
    public int Length => ByteCode.Length;

    /// <inheritdoc/>
    public bool Equals(EVMByteCode? other)
        => other is not null && ByteCode.Equals(other.ByteCode);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is EVMByteCode other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => ByteCode.GetHashCode();

    /// <summary>Checks whether two values refer to the same bytecode memory and slice.</summary>
    public static bool operator ==(EVMByteCode? left, EVMByteCode? right)
        => ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    /// <summary>Checks whether two values refer to different bytecode memory or slices.</summary>
    public static bool operator !=(EVMByteCode? left, EVMByteCode? right)
        => !(left == right);

    /// <summary>
    /// Checks if the given opcode exists in the contract code, ignoring bytes used as PUSH data and compiler metadata.
    /// </summary>
    /// <param name="opcode">The opcode to find.</param>
    /// <returns>True if the opcode appears outside PUSH data; otherwise false.</returns>
    public bool ContainsOpcode(EvmOpcode opcode)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;

        for(int i = 0; i < byteCode.Length; i++)
        {
            byte currentOpcode = byteCode[i];

            if(currentOpcode == (byte) opcode)
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
    public bool HasFunctions<TFunctionsSection>()
        where TFunctionsSection : IFunctionsSection
        => HasFunctions(TFunctionsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the function from the given generated function type.
    /// </summary>
    /// <typeparam name="TFunction"></typeparam>
    /// <returns></returns>
    public bool HasFunction<TFunction>()
        where TFunction : IGeneratedFunction
        => HasFunction(TFunction.Selector);

    /// <summary>
    /// Heuristically checks for the generated function's selector in PUSH constants.
    /// </summary>
    /// <typeparam name="TFunction"></typeparam>
    /// <returns></returns>
    public bool ContainsCallSelector<TFunction>()
        where TFunction : IGeneratedFunction
        => ContainsCallSelector(TFunction.Selector);

    /// <summary>
    /// Heuristically checks for a function selector in PUSH constants.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public bool ContainsCallSelector(Bytes4 selector)
        => HasPushedSelector(selector);

    /// <summary>
    /// Heuristically checks for all given function selectors in PUSH constants.
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public bool ContainsCallSelector(params ReadOnlySpan<Bytes4> selectors)
    {
        foreach(var selector in selectors)
        {
            if(!ContainsCallSelector(selector))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the given contract code implements a set of function selectors
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public bool HasFunctions(IEnumerable<ReadOnlyMemory<byte>> selectors)
    {
        foreach(var requiredSelector in selectors)
        {
            if(!HasFunctionSelector(requiredSelector.Span))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the given contract code implements all given function selectors.
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public bool HasFunctions(params ReadOnlySpan<Bytes4> selectors)
    {
        foreach(var selector in selectors)
        {
            if(!HasFunction(selector))
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
    public bool HasFunction(Bytes4 selector)
    {
        Span<byte> selectorBytes = stackalloc byte[Bytes4.BYTE_LENGTH];
        selector.CopyTo(selectorBytes);
        return HasFunctionSelector(selectorBytes);
    }

    /// <summary>
    /// Checks if the given contract code implements the events from the given logs section.
    /// </summary>
    /// <typeparam name="TLogsSection"></typeparam>
    /// <returns></returns>
    public bool HasEvents<TLogsSection>()
        where TLogsSection : ILogsSection
        => HasEvents(TLogsSection.GetTopics());

    /// <summary>
    /// Checks if the given contract code implements the event from the given generated log type.
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <returns></returns>
    public bool HasEvent<TLog>()
        where TLog : IGeneratedLog
        => HasEvent(TLog.Topic);

    /// <summary>
    /// Checks if the given contract code implements a set of event topics
    /// </summary>
    /// <param name="topics"></param>
    /// <returns></returns>
    public bool HasEvents(IEnumerable<ReadOnlyMemory<byte>> topics)
    {
        foreach(var requiredTopic in topics)
        {
            if(!HasEventTopic(requiredTopic.Span))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the given contract code implements all given event topics.
    /// </summary>
    /// <param name="topics"></param>
    /// <returns></returns>
    public bool HasEvents(params ReadOnlySpan<Bytes32> topics)
    {
        foreach(var topic in topics)
        {
            if(!HasEvent(topic))
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
    public bool HasEvent(Bytes32 topic)
    {
        Span<byte> topicBytes = stackalloc byte[Bytes32.BYTE_LENGTH];
        topic.CopyTo(topicBytes);
        return HasEventTopic(topicBytes);
    }

    /// <summary>
    /// Checks if the given contract code implements the errors from the given errors section.
    /// </summary>
    /// <typeparam name="TErrorsSection"></typeparam>
    /// <returns></returns>
    public bool HasErrors<TErrorsSection>()
        where TErrorsSection : IErrorsSection
        => HasErrors(TErrorsSection.GetSelectors());

    /// <summary>
    /// Checks if the given contract code implements the error from the given generated error type.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public bool HasError<TError>()
        where TError : ISolidityError<TError>
        => HasError(TError.Selector);

    /// <summary>
    /// Checks if the given contract code implements a set of error signatures.
    /// </summary>
    /// <param name="signatures"></param>
    /// <returns></returns>
    public bool HasErrors(IEnumerable<Bytes4> signatures)
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
    /// Checks if the given contract code implements all given error signatures.
    /// </summary>
    /// <param name="signatures"></param>
    /// <returns></returns>
    public bool HasErrors(params ReadOnlySpan<Bytes4> signatures)
    {
        foreach(var signature in signatures)
        {
            if(!HasError(signature))
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
    public bool HasError(Bytes4 signature)
        => HasPushedSelector(signature);

    private bool HasFunctionSelector(ReadOnlySpan<byte> selector)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;
        var normalizedSelector = selector.TrimStart((byte) 0);
        normalizedSelector = normalizedSelector.IsEmpty ? selector : normalizedSelector;

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

    private bool HasEventTopic(ReadOnlySpan<byte> topic)
    {
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;

        if(byteCode.IndexOf(topic) != -1)
        {
            return true;
        }

        var normalizedTopic = topic.TrimStart((byte) 0);
        normalizedTopic = normalizedTopic.IsEmpty ? topic : normalizedTopic;

        return normalizedTopic.Length != topic.Length
            && byteCode.IndexOf(normalizedTopic) != -1;
    }

    private bool HasPushedSelector(Bytes4 signature)
    {
        Span<byte> signatureBytes = stackalloc byte[Bytes4.BYTE_LENGTH];
        signature.CopyTo(signatureBytes);
        var byteCode = EvmBytecodeMetadata.GetExecutableByteCode(ByteCode).Span;
        uint selector = BinaryPrimitives.ReadUInt32BigEndian(signatureBytes);
        selector >>= BitOperations.TrailingZeroCount(selector);
        int selectorBits = Math.Max(1, 32 - BitOperations.LeadingZeroCount(selector));
        ulong mask = (1UL << selectorBits) - 1;

        for(int i = 0; i < byteCode.Length; i++)
        {
            if(byteCode[i] == (byte) EvmOpcode.Push0 && selector == 0)
            {
                return true;
            }

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

            // Search significant bits so byte padding, partial shifts, and optimized factoring all match.
            ulong value = 0;
            int bitCount = 0;
            foreach(byte currentByte in pushData)
            {
                value = (value << 8) | currentByte;
                bitCount += 8;
                for(int shift = 0; shift < 8 && shift <= bitCount - selectorBits; shift++)
                {
                    if(((value >> shift) & mask) == selector)
                    {
                        return true;
                    }
                }
            }

            i += pushLength;
        }

        return false;
    }
}
