using System.Buffers;

namespace EtherSharp.Common;

internal static class HexUtils
{
    public static bool TryDecodeHex(ReadOnlySpan<char> hex, Span<byte> destination)
    {
        var status = Convert.FromHexString(hex, destination, out int charsConsumed, out int bytesWritten);
        return status == OperationStatus.Done && charsConsumed == hex.Length && bytesWritten == destination.Length;
    }

    public static bool TryDecodeHex(ReadOnlySpan<byte> utf8Hex, Span<byte> destination)
    {
        var status = Convert.FromHexString(utf8Hex, destination, out int bytesConsumed, out int bytesWritten);
        return status == OperationStatus.Done && bytesConsumed == utf8Hex.Length && bytesWritten == destination.Length;
    }

    public static string ToPrefixedHexString(ReadOnlySpan<byte> data)
        => String.Create((data.Length * 2) + 2, data, static (span, state) =>
        {
            span[0] = '0';
            span[1] = 'x';
            if(!Convert.TryToHexStringLower(state, span[2..], out _))
            {
                throw new InvalidOperationException("Failed to write hex string.");
            }
        });
}
