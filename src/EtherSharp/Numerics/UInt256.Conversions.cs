// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    public string ToString()
        => ((BigInteger) this).ToString();
    public string ToString(string? format)
        => ((BigInteger) this).ToString(format);
    public string ToString(IFormatProvider? provider)
        => ((BigInteger) this).ToString(provider);

    public static bool TryParseFromHex(ReadOnlySpan<char> value, out UInt256 result)
    {
        if(value.Length > 64)
        {
            throw new ArgumentException("Value too long", nameof(value));
        }

        Span<byte> buffer = stackalloc byte[32];

        var status = Convert.FromHexString(value, buffer, out int charsConsumed, out int bytesWritten);

        if(status != System.Buffers.OperationStatus.Done)
        {
            throw new ArgumentException($"Hex parsing failed: {status}", nameof(value));
        }

        if(bytesWritten == 32)
        {
            result = BinaryPrimitives.ReadUInt256BigEndian(buffer);
            return true;
        }
        else if(bytesWritten == 0)
        {
            result = 0;
            return true;
        }

        Span<byte> prefixedBuffer = stackalloc byte[32];
        buffer[0..bytesWritten].CopyTo(prefixedBuffer[(32 - bytesWritten)..]);

        result = BinaryPrimitives.ReadUInt256BigEndian(prefixedBuffer);
        return true;
    }
}