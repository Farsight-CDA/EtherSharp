namespace EtherSharp.Generator.Util;

internal class HexUtils
{
    private const string HEX_ALPHABET = "0123456789ABCDEF";

    public static string ToHexString(Span<byte> bytes)
    {
        if(bytes.Length == 0)
        {
            return String.Empty;
        }

        char[] hexChars = new char[bytes.Length * 2];

        for(int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            hexChars[i * 2] = HEX_ALPHABET[b >> 4];
            hexChars[(i * 2) + 1] = HEX_ALPHABET[b & 0x0F];
        }

        return new string(hexChars);
    }

    public static byte[] FromHex(string hex)
    {
        var span = hex.AsSpan();

        if(span.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            span = span.Slice(2);
        }

        if(span.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string length is wrong.");
        }

        byte[] result = new byte[span.Length / 2];

        for(int i = 0; i < result.Length; i++)
        {
            result[i] = (byte) (
                (GetHexValue(span[i * 2]) << 4) | GetHexValue(span[(i * 2) + 1])
            );
        }

        return result;
    }

    private static int GetHexValue(char c)
        => c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'F' => c - 'A' + 10,
            >= 'a' and <= 'f' => c - 'a' + 10,
            _ => throw new ArgumentException($"Input contains garbage characters: '{c}'")
        };
}
