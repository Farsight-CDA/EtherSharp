using EtherSharp.Numerics;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace EtherSharp.Common.Converters.ComponentModel;

/// <summary>
/// Converts hexadecimal strings and integral values to <see cref="UInt256"/> values.
/// </summary>
public sealed class UInt256TypeConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string)
            || sourceType == typeof(byte)
            || sourceType == typeof(sbyte)
            || sourceType == typeof(ushort)
            || sourceType == typeof(short)
            || sourceType == typeof(uint)
            || sourceType == typeof(int)
            || sourceType == typeof(ulong)
            || sourceType == typeof(long)
            || sourceType == typeof(UInt128)
            || sourceType == typeof(Int128)
            || sourceType == typeof(BigInteger);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value switch
        {
            string hex => ParseHex(hex),
            byte number => (UInt256) number,
            sbyte number => (UInt256) number,
            ushort number => (UInt256) number,
            short number => (UInt256) number,
            uint number => (UInt256) number,
            int number => (UInt256) number,
            ulong number => (UInt256) number,
            long number => (UInt256) number,
            UInt128 number => (UInt256) (BigInteger) number,
            Int128 number => (UInt256) (BigInteger) number,
            BigInteger number => (UInt256) number,
            _ => throw GetConvertFromException(value),
        };

    private static UInt256 ParseHex(string value)
    {
        ReadOnlySpan<char> hex = value;
        if(hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[2..];
        }

        return UInt256.TryParseFromHex(hex, out var result)
            ? result
            : throw new FormatException($"Failed parsing {nameof(UInt256)}.");
    }
}
