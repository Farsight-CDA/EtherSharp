using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Numerics;

internal static class HexParser
{
    internal static bool TryParse(ReadOnlySpan<char> value, out UInt256 result)
    {
        result = default;

        if(value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            value = value[2..];
        }

        if(value.Length is < 1 or > 64)
        {
            return false;
        }

        Span<byte> buffer = stackalloc byte[32];
        int offset = 32 - ((value.Length + 1) / 2);
        buffer[..offset].Clear();

        if(value.Length % 2 != 0)
        {
            char first = value[0];
            if(!Char.IsAsciiHexDigit(first))
            {
                return false;
            }

            buffer[offset++] = (byte) (first <= '9' ? first - '0' : (first | 0x20) - 'a' + 10);
            value = value[1..];
        }

        if(Convert.FromHexString(value, buffer[offset..], out _, out _) != OperationStatus.Done)
        {
            return false;
        }

        result = BinaryPrimitives.ReadUInt256BigEndian(buffer);
        return true;
    }
}
