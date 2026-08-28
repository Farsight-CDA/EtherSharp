using EtherSharp.Numerics;
using EtherSharp.Types;
using System.ComponentModel;

namespace EtherSharp.Common.Converters.ComponentModel;

/// <summary>
/// Registers EtherSharp component model converters.
/// </summary>
public static class EtherSharpComponentModelConverters
{
    /// <summary>
    /// Registers all EtherSharp component model converters.
    /// </summary>
    public static void Register()
    {
        TypeDescriptor.AddAttributes(typeof(Address), new TypeConverterAttribute(typeof(AddressTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(Int256), new TypeConverterAttribute(typeof(Int256TypeConverter)));
        TypeDescriptor.AddAttributes(typeof(UInt256), new TypeConverterAttribute(typeof(UInt256TypeConverter)));
    }
}
