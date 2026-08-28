using EtherSharp.Types;
using System.ComponentModel;
using System.Globalization;

namespace EtherSharp.Common.Converters.ComponentModel;

/// <summary>
/// Converts string representations of Ethereum addresses to <see cref="Address"/> values.
/// </summary>
public sealed class AddressTypeConverter : TypeConverter
{
    /// <summary>
    /// Registers this converter for <see cref="Address"/> values.
    /// </summary>
    public static void Register()
        => TypeDescriptor.AddAttributes(typeof(Address), new TypeConverterAttribute(typeof(AddressTypeConverter)));

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string address
            ? Address.Parse(address)
            : throw GetConvertFromException(value);
}
