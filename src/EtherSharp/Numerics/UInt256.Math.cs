using System.Runtime.CompilerServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Computes the remainder of the exact sum of two values modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>The sum is not truncated to 256 bits before reduction.</remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddMod(in UInt256 a, in UInt256 b, in UInt256 m, out UInt256 result)
    {
        CoreUInt256.AddMod(AsUpstream(in a), AsUpstream(in b), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Computes the remainder of the exact product of two values modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>The product is not truncated to 256 bits before reduction.</remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MultiplyMod(in UInt256 a, in UInt256 b, in UInt256 m, out UInt256 result)
    {
        CoreUInt256.MultiplyMod(AsUpstream(in a), AsUpstream(in b), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Raises <paramref name="b"/> to the power <paramref name="e"/> modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>
    /// Intermediate products are not truncated to 256 bits before reduction.
    /// A zero exponent returns one modulo <paramref name="m"/>, including when the base is zero.
    /// </remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExpMod(in UInt256 b, in UInt256 e, in UInt256 m, out UInt256 result)
    {
        CoreUInt256.ExpMod(AsUpstream(in b), AsUpstream(in e), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Divides two values, storing the integer quotient in <paramref name="result"/>.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        CoreUInt256.Divide(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Computes the remainder of dividing <paramref name="a"/> by <paramref name="b"/>.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remainder(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        CoreUInt256.Mod(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Negates a value, storing the result modulo 2^256 in <paramref name="result"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Negate(in UInt256 value, out UInt256 result)
    {
        CoreUInt256.Subtract(CoreUInt256.Zero, AsUpstream(in value), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Multiplies two values, storing the product modulo 2^256 in <paramref name="result"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        CoreUInt256.Multiply(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Multiplies two values, storing the full 512-bit product in its low and high 256-bit halves.
    /// </summary>
    /// <remarks>The exact product is <paramref name="low"/> + <paramref name="high"/> * 2^256.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in UInt256 a, in UInt256 b, out UInt256 low, out UInt256 high)
    {
        Unsafe.SkipInit(out low);
        Unsafe.SkipInit(out high);
        CoreUInt256.Multiply256To512Bit(
            AsUpstream(in a), AsUpstream(in b),
            out Unsafe.As<UInt256, CoreUInt256>(ref low),
            out Unsafe.As<UInt256, CoreUInt256>(ref high)
        );
    }

    /// <summary>
    /// Multiplies two values, checking whether the product exceeds <see cref="MaxValue"/>.
    /// </summary>
    /// <param name="a">The first factor.</param>
    /// <param name="b">The second factor.</param>
    /// <param name="result">The exact product when no overflow occurs; otherwise, unspecified.</param>
    /// <returns><see langword="true"/> if the product overflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MultiplyOverflow(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        // Two factors of at most 128 bits always fit; avoid counting their bits.
        if((a._u2 | a._u3 | b._u2 | b._u3) == 0)
        {
            Multiply(in a, in b, out result);
            return false;
        }

        int bitLengthSum = a.GetShortestBitLength() + b.GetShortestBitLength();
        if(bitLengthSum <= 256)
        {
            Multiply(in a, in b, out result);
            return false;
        }

        // Nonzero factors have a product bit length of either sum - 1 or sum.
        if(bitLengthSum > 257)
        {
            result = default;
            return true;
        }

        Multiply(in a, in b, out result, out var high);
        // Match the scalar limb stores from the full-width multiplication.
        return (high._u0 | high._u1 | high._u2 | high._u3) != 0;
    }

    /// <summary>
    /// Raises <paramref name="b"/> to the power <paramref name="e"/>, returning the result modulo 2^256.
    /// </summary>
    /// <remarks>A zero exponent returns one, including when the base is zero.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 Pow(in UInt256 b, in UInt256 e)
    {
        CoreUInt256.Exp(AsUpstream(in b), AsUpstream(in e), out var result);
        return FromUpstream(result);
    }

    /// <summary>
    /// Returns the absolute difference between two values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 AbsoluteDifference(in UInt256 a, in UInt256 b)
    {
        CoreUInt256 result;
        if(AsUpstream(in a) < AsUpstream(in b))
        {
            CoreUInt256.Subtract(AsUpstream(in b), AsUpstream(in a), out result);
        }
        else
        {
            CoreUInt256.Subtract(AsUpstream(in a), AsUpstream(in b), out result);
        }

        return FromUpstream(result);
    }

    /// <summary>
    /// Returns the smaller of two values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 Min(in UInt256 a, in UInt256 b)
        => FromUpstream(CoreUInt256.Min(AsUpstream(in a), AsUpstream(in b)));

    /// <summary>
    /// Returns the larger of two values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 Max(in UInt256 a, in UInt256 b)
        => FromUpstream(CoreUInt256.Max(AsUpstream(in a), AsUpstream(in b)));

    /// <summary>
    /// Clamps a value to the inclusive range from <paramref name="min"/> to <paramref name="max"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 Clamp(in UInt256 value, in UInt256 min, in UInt256 max)
        => value switch
        {
            _ when AsUpstream(in max) < AsUpstream(in min)
                => throw new ArgumentException("The minimum must not exceed the maximum.", nameof(min)),
            _ when AsUpstream(in value) < AsUpstream(in min) => min,
            _ when AsUpstream(in max) < AsUpstream(in value) => max,
            _ => value
        };

    /// <summary>
    /// Adds two values, storing the sum modulo 2^256 in <paramref name="result"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the sum overflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Add(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        bool overflow = CoreUInt256.AddOverflow(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
        return overflow;
    }

    /// <summary>
    /// Subtracts two values, storing the difference modulo 2^256 in <paramref name="result"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the difference underflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Subtract(in UInt256 a, in UInt256 b, out UInt256 result)
    {
        bool overflow = CoreUInt256.SubtractUnderflow(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
        return overflow;
    }
}
