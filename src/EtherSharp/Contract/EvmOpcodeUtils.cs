namespace EtherSharp.Contract;

internal static class EvmOpcodeUtils
{
    public const byte PUSH1 = 0x60;
    public const byte PUSH4 = 0x63;
    public const byte PUSH32 = 0x7F;
    public const byte LT = 0x10;
    public const byte GT = 0x11;
    public const byte EQ = 0x14;
    public const byte SUB = 0x03;

    public static ReadOnlySpan<byte> ComparisonOpcodes => [LT, GT, EQ, SUB];

    public static bool TryGetPushLength(byte opcode, out int pushLength)
    {
        if(opcode is >= PUSH1 and <= PUSH32)
        {
            pushLength = opcode - (PUSH1 - 1);
            return true;
        }

        pushLength = 0;
        return false;
    }
}
