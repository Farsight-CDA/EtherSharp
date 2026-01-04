namespace EtherSharp.Common;

internal static class HexUtils
{
    public static string ToPrefixedHexString(ReadOnlySpan<byte> data)
        => String.Create((data.Length * 2) + 2, data, (span, state) =>
        {
            span[0] = '0';
            span[1] = 'x';
            if(!Convert.TryToHexString(state, span[2..], out _))
            {
                throw new InvalidOperationException("Failed to write hex string.");
            }
        });
}
