namespace EtherSharp.Contract;

/// <summary>Identifies PUSH immediate bytes, excluding the PUSH instructions themselves.</summary>
internal readonly struct PushDataMap
{
    private readonly byte[] _bits;

    public PushDataMap(ReadOnlySpan<byte> code)
    {
        _bits = new byte[(code.Length / 8) + (code.Length % 8 == 0 ? 0 : 1)];
        int position = 0;
        while(position < code.Length)
        {
            if(!EvmOpcodeUtils.TryGetPushLength(code[position++], out int pushLength))
            {
                continue;
            }

            int end = position + Math.Min(pushLength, code.Length - position);
            while(position < end)
            {
                int bitOffset = position % 8;
                int count = Math.Min(8 - bitOffset, end - position);
                _bits[position / 8] |= (byte) (((1 << count) - 1) << bitOffset);
                position += count;
            }
        }
    }

    /// <summary>Tests an offset already checked against the original code length.</summary>
    public bool IsPushData(int offset)
        => (_bits[offset / 8] & (1 << (offset % 8))) != 0;
}
