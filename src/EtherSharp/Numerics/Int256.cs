using EtherSharp.Common.Converters.Json;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace EtherSharp.Numerics;

/// <summary>
/// Represents a signed 256-bit integer with two's-complement semantics.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[JsonConverter(typeof(Int256HexConverter))]
public readonly partial struct Int256
{
    /// <summary>
    /// Represents the value zero.
    /// </summary>
    public static readonly Int256 Zero = default;

    /// <summary>
    /// Represents the value one.
    /// </summary>
    public static readonly Int256 One = new Int256(UInt256.One);

    /// <summary>
    /// Represents the fixed-point scaling factor 10^18.
    /// </summary>
    public static readonly Int256 WAD = new Int256(UInt256.WAD);

    /// <summary>
    /// Represents the fixed-point scaling factor 10^27.
    /// </summary>
    public static readonly Int256 RAY = new Int256(UInt256.RAY);

    /// <summary>
    /// Represents the smallest possible value, -2^255.
    /// </summary>
    public static readonly Int256 MinValue = new Int256(new UInt256(0, 0, 0, 0x8000000000000000UL));

    /// <summary>
    /// Represents the largest possible value, 2^255 - 1.
    /// </summary>
    public static readonly Int256 MaxValue = new Int256(new UInt256(UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue, Int64.MaxValue));

    internal readonly UInt256 _value;

    /// <summary>
    /// Whether the value is zero.
    /// </summary>
    public bool IsZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AsUpstream(in this).IsZero;
    }

    /// <summary>
    /// Whether the value is one.
    /// </summary>
    public bool IsOne
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AsUpstream(in this).IsOne;
    }

    /// <summary>
    /// Whether the value is negative.
    /// </summary>
    public bool IsNegative
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked((long) _value._u3) < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Int256(UInt256 value)
    {
        _value = value;
    }

    /// <summary>
    /// Counts the leading zero bits in the two's-complement representation.
    /// </summary>
    /// <returns>The number of leading zero bits, or 256 if the value is zero. Negative values return zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(in Int256 value)
        => UInt256.LeadingZeroCount(in value._value);

    /// <summary>
    /// Counts the trailing zero bits in the two's-complement representation.
    /// </summary>
    /// <returns>The number of trailing zero bits, or 256 if the value is zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(in Int256 value)
        => UInt256.TrailingZeroCount(in value._value);

    /// <summary>
    /// Gets the length, in bits, of the shortest two's-complement representation of the current value.
    /// </summary>
    /// <returns>
    /// Zero for zero; the number of significant bits for positive values;
    /// or the number of significant bits in the one's complement plus one sign bit for negative values.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetShortestBitLength()
        => IsNegative
            ? (~UInt256.AsUpstream(in _value)).BitLen + 1
            : _value.GetShortestBitLength();
}
