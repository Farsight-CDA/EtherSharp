using System.Runtime.CompilerServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
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
