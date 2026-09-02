namespace EtherSharp.Contract;

internal static class EvmOpcodeUtils
{
    public static ReadOnlySpan<byte> ComparisonOpcodes
        => [(byte) EvmOpcode.Lt, (byte) EvmOpcode.Gt, (byte) EvmOpcode.Eq, (byte) EvmOpcode.Sub];

    public static bool TryGetPushLength(byte opcode, out int pushLength)
    {
        if(opcode is >= (byte) EvmOpcode.Push1 and <= (byte) EvmOpcode.Push32)
        {
            pushLength = opcode - ((byte) EvmOpcode.Push1 - 1);
            return true;
        }

        pushLength = 0;
        return false;
    }
}
