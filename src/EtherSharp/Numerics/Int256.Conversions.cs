// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#pragma warning disable CS1591

using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    public override string ToString()
        => ((BigInteger) this).ToString();
    public string ToString(string? format)
        => ((BigInteger) this).ToString(format);
    public string ToString(IFormatProvider? provider)
        => ((BigInteger) this).ToString(provider);

    public static bool TryParseFromHex(ReadOnlySpan<char> value, out Int256 result)
    {
        result = default;

        if(value.Length > 64)
        {
            return false;
        }

        Span<byte> buffer = stackalloc byte[32];

        var status = Convert.FromHexString(value, buffer, out _, out int bytesWritten);

        if(status != System.Buffers.OperationStatus.Done)
        {
            return false;
        }

        if(bytesWritten == 32)
        {
            result = BinaryPrimitives.ReadInt256BigEndian(buffer);
            return true;
        }
        else if(bytesWritten == 0)
        {
            result = 0;
            return true;
        }

        Span<byte> prefixedBuffer = stackalloc byte[32];
        buffer[0..bytesWritten].CopyTo(prefixedBuffer[(32 - bytesWritten)..]);

        result = BinaryPrimitives.ReadInt256BigEndian(prefixedBuffer);
        return true;
    }
}
