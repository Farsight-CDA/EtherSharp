using EtherSharp.Common.Converters.Json;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

/// <summary>
/// Represents an unsigned 256-bit integer.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[JsonConverter(typeof(UInt256HexConverter))]
public readonly partial struct UInt256
{
    /// <summary>
    /// Represents the value zero.
    /// </summary>
    public static readonly UInt256 Zero = default;

    /// <summary>
    /// Represents the value one.
    /// </summary>
    public static readonly UInt256 One = new UInt256(1);

    /// <summary>
    /// Represents the fixed-point scaling factor 10^18.
    /// </summary>
    public static readonly UInt256 WAD = Pow(10, 18);

    /// <summary>
    /// Represents the fixed-point scaling factor 10^27.
    /// </summary>
    public static readonly UInt256 RAY = Pow(10, 27);

    /// <summary>
    /// Represents the smallest possible value, zero.
    /// </summary>
    public static readonly UInt256 MinValue = default;

    /// <summary>
    /// Represents the largest possible value, 2^256 - 1.
    /// </summary>
    public static readonly UInt256 MaxValue = new UInt256(UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue);

    internal readonly ulong _u0;
    internal readonly ulong _u1;
    internal readonly ulong _u2;
    internal readonly ulong _u3;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal UInt256(ulong u0, ulong u1 = 0, ulong u2 = 0, ulong u3 = 0)
    {
        _u0 = u0;
        _u1 = u1;
        _u2 = u2;
        _u3 = u3;
    }

    /// <summary>
    /// Counts the leading zero bits.
    /// </summary>
    /// <returns>The number of leading zero bits, or 256 if the value is zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(in UInt256 value)
        => 256 - AsUpstream(in value).BitLen;

    /// <summary>
    /// Counts the trailing zero bits.
    /// </summary>
    /// <returns>The number of trailing zero bits, or 256 if the value is zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(in UInt256 value)
        => value._u0 != 0
            ? BitOperations.TrailingZeroCount(value._u0)
            : value._u1 != 0
                ? 64 + BitOperations.TrailingZeroCount(value._u1)
                : value._u2 != 0
                    ? 128 + BitOperations.TrailingZeroCount(value._u2)
                    : 192 + BitOperations.TrailingZeroCount(value._u3);

    /// <summary>
    /// Gets the number of significant bits in the current value.
    /// </summary>
    /// <returns>The number of significant bits, or zero if the value is zero.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetShortestBitLength()
        => AsUpstream(in this).BitLen;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref readonly CoreUInt256 AsUpstream(in UInt256 value)
        => ref Unsafe.As<UInt256, CoreUInt256>(ref Unsafe.AsRef(in value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static UInt256 FromUpstream(CoreUInt256 value)
        => Unsafe.BitCast<CoreUInt256, UInt256>(value);
}
