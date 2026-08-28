using EtherSharp.Numerics;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace EtherSharp.Common.Converters.ComponentModel;

/// <summary>
/// Converts hexadecimal strings and integral values to <see cref="Int256"/> values.
/// </summary>
public sealed class Int256TypeConverter : TypeConverter
{
    /// <summary>
    /// Registers this converter for <see cref="Int256"/> values.
    /// </summary>
    public static void Register()
        => TypeDescriptor.AddAttributes(typeof(Int256), new TypeConverterAttribute(typeof(Int256TypeConverter)));

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
            byte number => (Int256) number,
            sbyte number => (Int256) number,
            ushort number => (Int256) number,
            short number => (Int256) number,
            uint number => (Int256) number,
            int number => (Int256) number,
            ulong number => (Int256) number,
            long number => (Int256) number,
            UInt128 number => (Int256) (BigInteger) number,
            Int128 number => (Int256) (BigInteger) number,
            BigInteger number => (Int256) number,
            _ => throw GetConvertFromException(value),
        };

    private static Int256 ParseHex(string value)
    {
        ReadOnlySpan<char> hex = value;
        if(hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[2..];
        }

        return Int256.TryParseFromHex(hex, out var result)
            ? result
            : throw new FormatException($"Failed parsing {nameof(Int256)}.");
    }
}
