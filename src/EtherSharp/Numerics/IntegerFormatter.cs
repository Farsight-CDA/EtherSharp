using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

// Decimal, culture and custom formats delegate to upstream, which falls back to BigInteger where it cannot format
// from the limbs. Two paths stay local because they are measurably faster: BCL formatting of values that fit
// 64 bits, and the radix writer.
internal static class IntegerFormatter
{
    // Covers every unpadded radix output (256 binary digits) and every unpadded decimal output.
    private const int STACK_BUFFER_LENGTH = 256;

    // Decimal digits of UInt256.MaxValue.
    private const int MAX_DECIMAL_DIGITS = 78;

    public static string Format(in UInt256 bits, bool isSigned, string? format, IFormatProvider? provider)
        => TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase)
            ? FormatRadix(in bits, format!, shift, precision, lowerCase)
            : FormatNonRadix(in bits, isSigned, format, provider);

    public static bool TryFormat(
        in UInt256 bits, bool isSigned, Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    ) => TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase)
        ? TryFormatRadix(in bits, destination, out charsWritten, shift, precision, lowerCase)
        : TryFormatNonRadix(in bits, isSigned, destination, out charsWritten, format, provider);

    public static bool TryFormat(
        in UInt256 bits, bool isSigned, Span<byte> destination, out int bytesWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        if(TryGetRadixFormat(format, out int shift, out int precision, out bool lowerCase))
        {
            return TryFormatRadix(in bits, destination, out bytesWritten, shift, precision, lowerCase);
        }

        if(IsPlainDecimalFormat(format) && FitsPrimitive(in bits, isSigned))
        {
            return isSigned
                ? ((long) bits._u0).TryFormat(destination, out bytesWritten, format, provider)
                : bits._u0.TryFormat(destination, out bytesWritten, format, provider);
        }

        // Upstream checks the length before writing only for non-negative decimal output; it writes negative
        // signs and BigInteger fallback output first, so those go through a scratch buffer unless they always fit.
        return IsDecimalFormat(format) && (!IsNegative(in bits, isSigned) || AlwaysFitsPlainDecimal(destination.Length, format, provider))
            ? CoreUInt256.TryFormatCore(UInt256.AsUpstream(in bits), isSigned, destination, out bytesWritten, UpstreamFormat(format), provider)
            : TryFormatBuffered(in bits, isSigned, destination, out bytesWritten, format, provider);
    }

    private static string FormatNonRadix(in UInt256 bits, bool isSigned, string? format, IFormatProvider? provider)
    {
        if(!IsPlainDecimalFormat(format))
        {
            return CoreUInt256.FormatToString(UInt256.AsUpstream(in bits), isSigned, format, provider);
        }

        if(FitsPrimitive(in bits, isSigned))
        {
            return isSigned
                ? ((long) bits._u0).ToString(format, provider)
                : bits._u0.ToString(format, provider);
        }

        // Unsigned digits do not depend on the culture; the D format preserves the culture's negative sign.
        return IsNegative(in bits, isSigned)
            ? CoreUInt256.FormatToString(UInt256.AsUpstream(in bits), isSigned, "D", provider)
            : UInt256.AsUpstream(in bits).ToString();
    }

    // Keep the radix entry point small enough to inline independently of decimal/custom dispatch.
    private static bool TryFormatNonRadix(
        in UInt256 bits, bool isSigned, Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    ) => !IsPlainDecimalFormat(format) || !FitsPrimitive(in bits, isSigned)
        ? CoreUInt256.TryFormatCore(UInt256.AsUpstream(in bits), isSigned, destination, out charsWritten, UpstreamFormat(format), provider)
        : isSigned
            ? ((long) bits._u0).TryFormat(destination, out charsWritten, format, provider)
            : bits._u0.TryFormat(destination, out charsWritten, format, provider);

    // Formats into scratch space and copies on success, so a destination that is too small is never written.
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TryFormatBuffered(
        in UInt256 bits, bool isSigned, Span<byte> destination, out int bytesWritten,
        ReadOnlySpan<char> format, IFormatProvider? provider
    )
    {
        Span<byte> scratch = stackalloc byte[Math.Min(STACK_BUFFER_LENGTH, destination.Length)];
        if(CoreUInt256.TryFormatCore(UInt256.AsUpstream(in bits), isSigned, scratch, out int written, UpstreamFormat(format), provider))
        {
            scratch[..written].CopyTo(destination);
            bytesWritten = written;
            return true;
        }

        bytesWritten = 0;
        if(destination.Length <= scratch.Length)
        {
            return false;
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(destination.Length);
        try
        {
            if(!CoreUInt256.TryFormatCore(UInt256.AsUpstream(in bits), isSigned, buffer.AsSpan(0, destination.Length), out written, UpstreamFormat(format), provider))
            {
                return false;
            }

            buffer.AsSpan(0, written).CopyTo(destination);
            bytesWritten = written;
            return true;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    // D preserves the culture's negative sign; upstream's empty format uses a literal '-'.
    private static ReadOnlySpan<char> UpstreamFormat(ReadOnlySpan<char> format)
        => format.IsEmpty ? "D" : format;

    // A UTF-16 character takes at most three UTF-8 bytes.
    private static bool AlwaysFitsPlainDecimal(int destinationLength, ReadOnlySpan<char> format, IFormatProvider? provider)
        => IsPlainDecimalFormat(format)
            && destinationLength >= MAX_DECIMAL_DIGITS + (3 * NumberFormatInfo.GetInstance(provider).NegativeSign.Length);

    private static bool IsNegative(in UInt256 bits, bool isSigned)
        => isSigned && (long) bits._u3 < 0;

    // Values that fit Int64 (signed) or UInt64 (unsigned) use the BCL formatter, which is faster and caches small strings.
    private static bool FitsPrimitive(in UInt256 bits, bool isSigned)
    {
        ulong extension = isSigned ? (ulong) ((long) bits._u0 >> 63) : 0;
        return bits._u1 == extension && bits._u2 == extension && bits._u3 == extension;
    }

    // Decimal without a precision; the BCL pads precision one division per digit, which upstream avoids.
    private static bool IsPlainDecimalFormat(ReadOnlySpan<char> format)
        => format.IsEmpty || (format.Length == 1 && format[0] is 'D' or 'd' or 'G' or 'g');

    // The formats upstream writes from the limbs, apart from the radix formats handled locally.
    private static bool IsDecimalFormat(ReadOnlySpan<char> format)
        => format.IsEmpty
            || (format[0] is 'D' or 'd'
                ? !format[1..].ContainsAnyExceptInRange('0', '9')
                : format.Length == 1 && format[0] is 'G' or 'g');

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

    private static int GetRadixDigits(in UInt256 bits, int shift)
        => Math.Max(1, (bits.GetShortestBitLength() + shift - 1) / shift);

    [SkipLocalsInit]
    private static string FormatRadix(in UInt256 bits, string format, int shift, int precision, bool lowerCase)
    {
        int digits = GetRadixDigits(in bits, shift);

        // UInt64 accepts the same radix formats, including '\0' termination, and skips the buffer copy.
        // It pads one digit per iteration, so leave zero padding to the local writer.
        if((bits._u1 | bits._u2 | bits._u3) == 0 && precision <= digits)
        {
            return bits._u0.ToString(format, CultureInfo.InvariantCulture);
        }

        int length = Math.Max(digits, precision);
        if(length > STACK_BUFFER_LENGTH)
        {
            return String.Create(
                length, (bits, digits, shift, lowerCase), static (destination, state) =>
                    WriteRadix(state.bits, destination, state.digits, state.shift, state.lowerCase)
            );
        }

        Span<char> buffer = stackalloc char[STACK_BUFFER_LENGTH];
        buffer = buffer[..length];
        WriteRadix(in bits, buffer, digits, shift, lowerCase);
        return new string(buffer);
    }

    private static bool TryFormatRadix<T>(
        in UInt256 bits, Span<T> destination, out int written, int shift, int precision, bool lowerCase
    ) where T : unmanaged, IBinaryInteger<T>
    {
        int digits = GetRadixDigits(in bits, shift);
        int length = Math.Max(digits, precision);
        written = 0;
        if(destination.Length < length)
        {
            return false;
        }

        WriteRadix(in bits, destination[..length], digits, shift, lowerCase);
        written = length;
        return true;
    }

    private static void WriteRadix<T>(in UInt256 bits, Span<T> destination, int digits, int shift, bool lowerCase)
        where T : unmanaged, IBinaryInteger<T>
    {
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

    // UInt64 formatting is cheaper than UInt128 and pads the same way, so use it when the upper half is zero.
    private static void WriteUInt128<T>(UInt128 value, Span<T> destination, ReadOnlySpan<char> format)
        where T : unmanaged
    {
        if(value <= UInt64.MaxValue)
        {
            ulong narrow = (ulong) value;
            _ = typeof(T) == typeof(char)
                ? narrow.TryFormat(MemoryMarshal.Cast<T, char>(destination), out _, format)
                : narrow.TryFormat(MemoryMarshal.Cast<T, byte>(destination), out _, format);
            return;
        }

        _ = typeof(T) == typeof(char)
            ? value.TryFormat(MemoryMarshal.Cast<T, char>(destination), out _, format)
            : value.TryFormat(MemoryMarshal.Cast<T, byte>(destination), out _, format);
    }
}
