using System.Buffers;

namespace EtherSharp.Common;

internal static class HexUtils
{
    public static ReadOnlySpan<char> StripHexPrefix(ReadOnlySpan<char> hex)
        => hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? hex[2..]
            : hex;

    public static ReadOnlySpan<byte> StripHexPrefix(ReadOnlySpan<byte> utf8Hex)
        => utf8Hex.StartsWith("0x"u8) || utf8Hex.StartsWith("0X"u8)
            ? utf8Hex[2..]
            : utf8Hex;

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
