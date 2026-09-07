using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace EtherSharp.Numerics;

internal static class IntegerFormatter
{
    public static string Format(in UInt256 bits, bool isSigned, string? format, IFormatProvider? provider)
    {
        if(TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase))
        {
            int length = GetRadixLength(in bits, shift, precision);
            return String.Create(
                length, (bits, shift, lowerCase), static (destination, state) =>
                    WriteRadix(state.bits, destination, state.shift, state.lowerCase)
            );
        }

        return ToBigInteger(in bits, isSigned).ToString(format, provider);
    }

    public static bool TryFormat(
        in UInt256 bits, bool isSigned, Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    ) => TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase)
        ? TryFormatRadix(in bits, destination, out charsWritten, shift, precision, lowerCase)
        : ToBigInteger(in bits, isSigned).TryFormat(destination, out charsWritten, format, provider);

    public static bool TryFormat(
        in UInt256 bits, bool isSigned, Span<byte> destination, out int bytesWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        if(TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase))
        {
            return TryFormatRadix(in bits, destination, out bytesWritten, shift, precision, lowerCase);
        }

        // BigInteger only formats UTF-16. Most output fits on the stack; longer custom formats
        // borrow a buffer bounded by the destination size (UTF-8 needs at least as many bytes).
        var value = ToBigInteger(in bits, isSigned);
        Span<char> chars = stackalloc char[Math.Min(256, destination.Length)];
        if(value.TryFormat(chars, out int charsWritten, format, provider))
        {
            return Encoding.UTF8.TryGetBytes(chars[..charsWritten], destination, out bytesWritten);
        }

        bytesWritten = 0;
        if(destination.Length <= chars.Length)
        {
            return false;
        }

        char[] buffer = ArrayPool<char>.Shared.Rent(destination.Length);
        try
        {
            return value.TryFormat(buffer.AsSpan(0, destination.Length), out charsWritten, format, provider)
                && Encoding.UTF8.TryGetBytes(buffer.AsSpan(0, charsWritten), destination, out bytesWritten);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static BigInteger ToBigInteger(in UInt256 bits, bool isSigned)
        => isSigned ? (BigInteger) new Int256(bits) : (BigInteger) bits;

    private static bool TryGetRadixFormat(ReadOnlySpan<char> format, out int shift, out int precision, out bool lowerCase)
    {
        shift = 0;
        precision = 0;
        lowerCase = false;
        if(format.IsEmpty || format[0] is not ('X' or 'x' or 'B' or 'b'))
        {
            return false;
        }

        for(int i = 1; i < format.Length && format[i] != '\0'; i++)
        {
            int digit = format[i] - '0';
            if((uint) digit > 9)
            {
                return false;
            }
            if(precision >= 100_000_000)
            {
                throw new FormatException("The numeric format precision is too large.");
            }
            precision = (precision * 10) + digit;
        }

        shift = format[0] is 'X' or 'x' ? 4 : 1;
        lowerCase = format[0] == 'x';
        return true;
    }

    private static int GetRadixLength(in UInt256 bits, int shift, int precision)
        => Math.Max(Math.Max(1, (bits.GetShortestBitLength() + shift - 1) / shift), precision);

    private static bool TryFormatRadix<T>(
        in UInt256 bits, Span<T> destination, out int written, int shift, int precision, bool lowerCase
    ) where T : unmanaged, IBinaryInteger<T>
    {
        int length = GetRadixLength(in bits, shift, precision);
        written = 0;
        if(destination.Length < length)
        {
            return false;
        }

        WriteRadix(in bits, destination[..length], shift, lowerCase);
        written = length;
        return true;
    }

    private static void WriteRadix<T>(in UInt256 bits, Span<T> destination, int shift, bool lowerCase)
        where T : unmanaged, IBinaryInteger<T>
    {
        int digits = GetRadixLength(in bits, shift, 0);
        destination[..^digits].Fill(T.CreateTruncating('0'));
        destination = destination[^digits..];

        var low = ((UInt128) bits._u1 << 64) | bits._u0;
        ReadOnlySpan<char> format = shift == 1 ? "B" : lowerCase ? "x" : "X";
        int halfDigits = 128 / shift;
        if(digits > halfDigits)
        {
            var high = ((UInt128) bits._u3 << 64) | bits._u2;
            WriteUInt128(high, destination[..^halfDigits], format);
            destination = destination[^halfDigits..];
            format = shift == 1 ? "B128" : lowerCase ? "x32" : "X32";
        }

        WriteUInt128(low, destination, format);
    }

    private static void WriteUInt128<T>(UInt128 value, Span<T> destination, ReadOnlySpan<char> format)
        where T : unmanaged
        => _ = typeof(T) == typeof(char)
            ? value.TryFormat(MemoryMarshal.Cast<T, char>(destination), out _, format)
            : value.TryFormat(MemoryMarshal.Cast<T, byte>(destination), out _, format);
}
