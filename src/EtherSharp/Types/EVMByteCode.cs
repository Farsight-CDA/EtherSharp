namespace EtherSharp.Types;

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

    private const byte PUSH4_OPCODE = 0x63;
    private const byte PUSH32_OPCODE = 0x7F;
    private const byte LT_OPCODE = 0x10;
    private const byte GT_OPCODE = 0x11;
    private const byte EQ_OPCODE = 0x14;
    private const byte SUB_OPCODE = 0x03;
    private static ReadOnlySpan<byte> ComparisonOpcodes => [LT_OPCODE, GT_OPCODE, EQ_OPCODE, SUB_OPCODE];

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
    /// Checks if the given contract code implements a set of function selectors
    /// </summary>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public readonly bool HasFunctions(params IEnumerable<ReadOnlyMemory<byte>> selectors)
    {
        Span<byte> prefixedSelector = stackalloc byte[5];
        foreach(var requiredSelector in selectors)
        {
            prefixedSelector[0] = PUSH4_OPCODE;
            requiredSelector.Span.CopyTo(prefixedSelector[1..]);

            int index = ByteCode.Span.IndexOf(prefixedSelector);

            if(index == -1 || ByteCode.Length < (index + 7))
            {
                return false;
            }

            byte suffixByte = ByteCode.Span[index + 5];

            if(!ComparisonOpcodes.Contains(suffixByte))
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
        Span<byte> prefixedSelector = stackalloc byte[33];
        foreach(var requiredTopic in topics)
        {
            prefixedSelector[0] = PUSH32_OPCODE;
            requiredTopic.Span.CopyTo(prefixedSelector[1..]);

            int index = ByteCode.Span.IndexOf(prefixedSelector);

            if(index == -1)
            {
                return false;
            }
        }

        return true;
    }
}