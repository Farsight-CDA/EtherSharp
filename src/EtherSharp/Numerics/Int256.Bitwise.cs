using System.Runtime.CompilerServices;
using CoreInt256 = Nethermind.Int256.Int256;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    /// <summary>
    /// Computes the bitwise AND of two 256-bit two's-complement representations.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void And(in Int256 a, in Int256 b, out Int256 result)
    {
        Unsafe.SkipInit(out result);
        CoreInt256.And(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Computes the bitwise OR of two 256-bit two's-complement representations.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Or(in Int256 a, in Int256 b, out Int256 result)
    {
        Unsafe.SkipInit(out result);
        CoreInt256.Or(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Computes the bitwise exclusive OR of two 256-bit two's-complement representations.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <param name="result">The result. May alias either operand.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Xor(in Int256 a, in Int256 b, out Int256 result)
    {
        Unsafe.SkipInit(out result);
        CoreInt256.Xor(AsUpstream(in a), AsUpstream(in b), out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Complements every bit of the 256-bit two's-complement representation.
    /// </summary>
    /// <param name="value">The value to complement.</param>
    /// <param name="result">The complemented value. May alias <paramref name="value"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Not(in Int256 value, out Int256 result)
    {
        Unsafe.SkipInit(out result);
        CoreInt256.Not(AsUpstream(in value), out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Shifts left, discarding bits shifted out of the two's-complement representation.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="n">The shift count.</param>
    /// <param name="result">The shifted value. May alias <paramref name="value"/>.</param>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftLeft(in Int256 value, int n, out Int256 result)
    {
        n &= 255;
        Unsafe.SkipInit(out result);
        CoreInt256.LeftShift(AsUpstream(in value), n, out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with the sign bit.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="n">The shift count.</param>
    /// <param name="result">The shifted value. May alias <paramref name="value"/>.</param>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftRightArithmetic(in Int256 value, int n, out Int256 result)
    {
        n &= 255;
        Unsafe.SkipInit(out result);
        CoreInt256.RightShift(AsUpstream(in value), n, out Unsafe.As<Int256, CoreInt256>(ref result));
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with zero regardless of the sign.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="n">The shift count.</param>
    /// <param name="result">The shifted value. May alias <paramref name="value"/>.</param>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftRightLogical(in Int256 value, int n, out Int256 result)
    {
        n &= 255;
        ref readonly var bits = ref Unsafe.As<Int256, CoreUInt256>(ref Unsafe.AsRef(in value));
        Unsafe.SkipInit(out result);
        CoreUInt256.Rsh(in bits, n, out Unsafe.As<Int256, CoreUInt256>(ref result));
    }
}
