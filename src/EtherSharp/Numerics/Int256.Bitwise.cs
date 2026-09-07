using System.Runtime.CompilerServices;
using CoreInt256 = Nethermind.Int256.Int256;

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
}
