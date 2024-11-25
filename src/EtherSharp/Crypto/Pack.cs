namespace EtherSharp.Crypto;
internal static class Pack
{
    private static void UInt32_To_LE(uint n, Span<byte> bs)
    {
        bs[0] = (byte) n;
        bs[1] = (byte) (n >> 8);
        bs[2] = (byte) (n >> 16);
        bs[3] = (byte) (n >> 24);
    }

    private static uint LE_To_UInt32(ReadOnlySpan<byte> bs)
        => bs[0]
            | ((uint) bs[1] << 8)
            | ((uint) bs[2] << 16)
            | ((uint) bs[3] << 24);

    private static void UInt64_To_LE(ulong n, Span<byte> bs)
    {
        UInt32_To_LE((uint) n, bs);
        UInt32_To_LE((uint) (n >> 32), bs[4..]);
    }

    internal static void UInt64_To_LE(ReadOnlySpan<ulong> ns, int nsLen, Span<byte> bs)
    {
        for(int i = 0; i < nsLen; ++i)
        {
            UInt64_To_LE(ns[i], bs);
            bs = bs[8..];
        }
    }

    internal static ulong LE_To_UInt64(ReadOnlySpan<byte> bs)
    {
        uint lo = LE_To_UInt32(bs);
        uint hi = LE_To_UInt32(bs[4..]);
        return ((ulong) hi << 32) | lo;
    }
}