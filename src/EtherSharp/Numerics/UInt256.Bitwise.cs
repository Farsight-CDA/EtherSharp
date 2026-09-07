using System.Runtime.CompilerServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Computes the bitwise AND of two 256-bit values.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void And(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        Unsafe.SkipInit(out result);
        CoreUInt256.And(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }

    /// <summary>
    /// Computes the bitwise OR of two 256-bit values.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Or(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        Unsafe.SkipInit(out result);
        CoreUInt256.Or(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }

    /// <summary>
    /// Computes the bitwise exclusive OR of two 256-bit values.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Xor(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        Unsafe.SkipInit(out result);
        CoreUInt256.Xor(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }

    /// <summary>
    /// Complements every bit of the 256-bit representation.
    /// </summary>
    /// <param name="value">The value to complement.</param>
    /// <param name="result">The complemented value. May alias <paramref name="value"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Not(in UInt256 value, out UInt256 result)
    {
        Unsafe.SkipInit(out result);
        CoreUInt256.Not(AsUpstream(in value), out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }

    /// <summary>
    /// Shifts left, discarding bits shifted out of the 256-bit range.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="n">The shift count.</param>
    /// <param name="result">The shifted value. May alias <paramref name="value"/>.</param>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftLeft(in UInt256 value, int n, out UInt256 result)
    {
        n &= 255;
        Unsafe.SkipInit(out result);
        CoreUInt256.Lsh(AsUpstream(in value), n, out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with zero.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="n">The shift count.</param>
    /// <param name="result">The shifted value. May alias <paramref name="value"/>.</param>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftRightLogical(in UInt256 value, int n, out UInt256 result)
    {
        n &= 255;
        Unsafe.SkipInit(out result);
        CoreUInt256.Rsh(AsUpstream(in value), n, out Unsafe.As<UInt256, CoreUInt256>(ref result));
    }
}
