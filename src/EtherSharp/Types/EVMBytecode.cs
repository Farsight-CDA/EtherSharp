namespace EtherSharp.Types;

/// <summary>
/// Represents the source code of an EVM contract.
/// </summary>
/// <param name="bytecode"></param>
public struct EVMBytecode(ReadOnlyMemory<byte> bytecode)
{
    private const byte PUSH4_OPCODE = 0x63;
    private const byte LT_OPCODE = 0x10;
    private const byte GT_OPCODE = 0x11;
    private const byte EQ_OPCODE = 0x14;
    private const byte SUB_OPCODE = 0x03;
    private static ReadOnlySpan<byte> ComparisonOpcodes => [LT_OPCODE, GT_OPCODE, EQ_OPCODE, SUB_OPCODE];

    /// <summary>
    /// Raw bytecode bytes.
    /// </summary>
    public ReadOnlyMemory<byte> Bytecode { get; } = bytecode;

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

            int index = Bytecode.Span.IndexOf(prefixedSelector);

            if(index == -1 || Bytecode.Length < (index + 7))
            {
                return false;
            }

            byte suffixByte = Bytecode.Span[index + 5];

            if(!ComparisonOpcodes.Contains(suffixByte))
            {
                return false;
            }
        }

        return true;
    }
}