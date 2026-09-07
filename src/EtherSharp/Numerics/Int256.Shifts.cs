using System.Runtime.CompilerServices;
using CoreInt256 = Nethermind.Int256.Int256;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
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
