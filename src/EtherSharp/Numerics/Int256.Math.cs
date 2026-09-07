using System.Runtime.CompilerServices;
using CoreInt256 = Nethermind.Int256.Int256;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    /// <summary>
    /// Computes the remainder of the exact sum of two values modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>
    /// The sum is not truncated to 256 bits before reduction.
    /// A nonzero remainder has the same sign as the exact sum; the sign of the modulus is ignored.
    /// </remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddMod(in Int256 a, in Int256 b, in Int256 m, out Int256 result)
    {
        CoreInt256.AddMod(AsUpstream(in a), AsUpstream(in b), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Computes the remainder of the exact difference of two values modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>
    /// The difference is not truncated to 256 bits before reduction.
    /// A nonzero remainder has the same sign as the exact difference; the sign of the modulus is ignored.
    /// </remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SubtractMod(in Int256 a, in Int256 b, in Int256 m, out Int256 result)
    {
        CoreInt256.SubtractMod(AsUpstream(in a), AsUpstream(in b), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Computes the remainder of the exact product of two values modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>
    /// The product is not truncated to 256 bits before reduction.
    /// A nonzero remainder has the same sign as the exact product; the sign of the modulus is ignored.
    /// </remarks>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MultiplyMod(in Int256 a, in Int256 b, in Int256 m, out Int256 result)
    {
        CoreInt256.MultiplyMod(AsUpstream(in a), AsUpstream(in b), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Raises <paramref name="b"/> to the power <paramref name="e"/> modulo <paramref name="m"/>.
    /// </summary>
    /// <remarks>
    /// Intermediate products are not truncated to 256 bits before reduction.
    /// A nonzero remainder is negative only when the base is negative and the exponent is odd; the sign of the modulus is ignored.
    /// A zero exponent returns one modulo <paramref name="m"/>, including when the base is zero.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="e"/> is negative.</exception>
    /// <exception cref="DivideByZeroException"><paramref name="m"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExpMod(in Int256 b, in Int256 e, in Int256 m, out Int256 result)
    {
        if(e.IsNegative)
        {
            throw new ArgumentOutOfRangeException(nameof(e), "The exponent must be non-negative.");
        }

        CoreInt256.ExpMod(AsUpstream(in b), AsUpstream(in e), AsUpstream(in m), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Divides two values, storing the quotient truncated toward zero in <paramref name="result"/>.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    /// <exception cref="OverflowException"><paramref name="a"/> is <see cref="MinValue"/> and <paramref name="b"/> is minus one, including in an unchecked context.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Divide(in Int256 a, in Int256 b, out Int256 result)
    {
        if(a == MinValue && b == -One)
        {
            throw new OverflowException();
        }

        CoreInt256.Divide(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Computes the remainder of dividing <paramref name="a"/> by <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// A nonzero remainder has the same sign as <paramref name="a"/>.
    /// </remarks>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    /// <exception cref="OverflowException"><paramref name="a"/> is <see cref="MinValue"/> and <paramref name="b"/> is minus one, including in an unchecked context.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remainder(in Int256 a, in Int256 b, out Int256 result)
    {
        if(a == MinValue && b == -One)
        {
            throw new OverflowException();
        }

        CoreInt256.Mod(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Negates a value, storing the wrapped two's-complement result in <paramref name="result"/>.
    /// </summary>
    /// <remarks>Negating <see cref="MinValue"/> wraps to <see cref="MinValue"/>.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Negate(in Int256 value, out Int256 result)
    {
        CoreInt256.Neg(AsUpstream(in value), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Multiplies two values, storing the wrapped two's-complement product in <paramref name="result"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in Int256 a, in Int256 b, out Int256 result)
    {
        CoreInt256.Multiply(AsUpstream(in a), AsUpstream(in b), out var coreResult);
        result = FromUpstream(coreResult);
    }

    /// <summary>
    /// Multiplies two signed values, storing the full 512-bit product in an unsigned low half and a signed high half.
    /// </summary>
    /// <remarks>The exact product is <paramref name="low"/> + <paramref name="high"/> * 2^256.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Multiply(in Int256 a, in Int256 b, out UInt256 low, out Int256 high)
    {
        UInt256.Multiply(in a._value, in b._value, out var lowBits, out var highBits);

        // Each negative operand represents its unsigned bits minus 2^256.
        // Correct the high half before writing outputs that might alias the inputs.
        if(a.IsNegative)
        {
            UInt256.Subtract(in highBits, in b._value, out highBits);
        }
        if(b.IsNegative)
        {
            UInt256.Subtract(in highBits, in a._value, out highBits);
        }

        low = lowBits;
        high = new Int256(highBits);
    }

    /// <summary>
    /// Multiplies two values, checking whether the product is outside the signed 256-bit range.
    /// </summary>
    /// <param name="a">The first factor.</param>
    /// <param name="b">The second factor.</param>
    /// <param name="result">The exact product when no overflow occurs; otherwise, unspecified.</param>
    /// <returns><see langword="true"/> if the product overflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MultiplyOverflow(in Int256 a, in Int256 b, out Int256 result)
    {
        if(a.IsZero || b.IsZero)
        {
            result = Zero;
            return false;
        }

        // Read the absolute values as unsigned bits so MinValue has magnitude 2^255.
        AsUpstream(in a).Abs(out var aMagnitude);
        AsUpstream(in b).Abs(out var bMagnitude);
        int aBitLength = aMagnitude._value.BitLen;
        int bBitLength = bMagnitude._value.BitLen;
        int bitLengthSum = aBitLength + bBitLength;
        if(bitLengthSum <= 255)
        {
            Multiply(in a, in b, out result);
            return false;
        }

        // A negative product may have magnitude exactly 2^255, unlike a positive product.
        int maxBitLengthSum = a.IsNegative != b.IsNegative ? 257 : 256;
        if(bitLengthSum > maxBitLengthSum)
        {
            result = default;
            return true;
        }

        Multiply(in a, in b, out var low, out var high);
        result = new Int256(low);
        var signExtension = result.IsNegative
            ? UInt256.MaxValue
            : UInt256.Zero;
        return high._value != signExtension;
    }

    /// <summary>
    /// Divides the exact product of two values by <paramref name="denominator"/>, checking whether the quotient is outside the signed 256-bit range.
    /// </summary>
    /// <param name="x">The first factor.</param>
    /// <param name="y">The second factor.</param>
    /// <param name="denominator">The divisor.</param>
    /// <param name="result">The quotient truncated toward zero, or zero if it overflows.</param>
    /// <returns><see langword="true"/> if the quotient overflows; otherwise, <see langword="false"/>.</returns>
    /// <remarks>The product is not truncated to 256 bits before division.</remarks>
    /// <exception cref="DivideByZeroException"><paramref name="denominator"/> is zero.</exception>
    public static bool MulDivOverflow(in Int256 x, in Int256 y, in Int256 denominator, out Int256 result)
    {
        bool negative = x.IsNegative ^ y.IsNegative ^ denominator.IsNegative;
        // Abs preserves the magnitude of MinValue in the unsigned backing bits.
        AsUpstream(in x).Abs(out var xMagnitude);
        AsUpstream(in y).Abs(out var yMagnitude);
        AsUpstream(in denominator).Abs(out var denominatorMagnitude);
        var unsignedX = UInt256.FromUpstream(xMagnitude._value);
        var unsignedY = UInt256.FromUpstream(yMagnitude._value);
        var unsignedDenominator = UInt256.FromUpstream(denominatorMagnitude._value);
        if(UInt256.MulDivOverflow(in unsignedX, in unsignedY, in unsignedDenominator, out var magnitude)
            || (magnitude > (negative ? MinValue._value : MaxValue._value)))
        {
            result = default;
            return true;
        }

        var quotient = new Int256(magnitude);
        result = negative ? -quotient : quotient;
        return false;
    }

    /// <summary>
    /// Raises <paramref name="b"/> to the power <paramref name="e"/> using wrapping two's-complement semantics.
    /// </summary>
    /// <remarks>A zero exponent returns one, including when the base is zero.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="e"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 Pow(in Int256 b, in Int256 e)
    {
        if(e.IsNegative)
        {
            throw new ArgumentOutOfRangeException(nameof(e), "The exponent must be non-negative.");
        }

        CoreInt256.Exp(AsUpstream(in b), AsUpstream(in e), out var result);
        return FromUpstream(result);
    }

    /// <summary>
    /// Returns the absolute value.
    /// </summary>
    /// <exception cref="OverflowException"><paramref name="value"/> is <see cref="MinValue"/>, including in an unchecked context.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 Abs(in Int256 value)
    {
        if(value == MinValue)
        {
            throw new OverflowException();
        }

        AsUpstream(in value).Abs(out var result);
        return FromUpstream(result);
    }

    /// <summary>
    /// Returns the absolute difference between two signed values as an unsigned value.
    /// </summary>
    /// <remarks>The result is exact, including the distance between <see cref="MinValue"/> and <see cref="MaxValue"/>.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 AbsoluteDifference(in Int256 a, in Int256 b)
    {
        // Subtract the ordered two's-complement bits modulo 2^256 to preserve the full unsigned distance.
        CoreUInt256 result;
        if(AsUpstream(in a) < AsUpstream(in b))
        {
            CoreUInt256.Subtract(AsUpstream(in b)._value, AsUpstream(in a)._value, out result);
        }
        else
        {
            CoreUInt256.Subtract(AsUpstream(in a)._value, AsUpstream(in b)._value, out result);
        }

        return UInt256.FromUpstream(result);
    }

    /// <summary>
    /// Returns the smaller of two signed values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 Min(in Int256 a, in Int256 b)
        => (AsUpstream(in b) < AsUpstream(in a))
            ? b
            : a;

    /// <summary>
    /// Returns the larger of two signed values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 Max(in Int256 a, in Int256 b)
        => (AsUpstream(in b) < AsUpstream(in a))
            ? a
            : b;

    /// <summary>
    /// Clamps a signed value to the inclusive range from <paramref name="min"/> to <paramref name="max"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 Clamp(in Int256 value, in Int256 min, in Int256 max)
        => value switch
        {
            _ when AsUpstream(in max) < AsUpstream(in min)
                => throw new ArgumentException("The minimum must not exceed the maximum.", nameof(min)),
            _ when AsUpstream(in value) < AsUpstream(in min) => min,
            _ when AsUpstream(in max) < AsUpstream(in value) => max,
            _ => value
        };

    /// <summary>
    /// Returns the value with the smaller absolute magnitude, preferring the negative value when magnitudes are equal.
    /// </summary>
    /// <remarks>The magnitude of <see cref="MinValue"/> is treated as 2^255 without overflow.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 MinMagnitude(in Int256 a, in Int256 b)
    {
        AsUpstream(in a).Abs(out var aMagnitude);
        AsUpstream(in b).Abs(out var bMagnitude);
        int comparison = aMagnitude._value.CompareTo(in bMagnitude._value);
        return (comparison < 0) || ((comparison == 0) && a.IsNegative) ? a : b;
    }

    /// <summary>
    /// Returns the value with the larger absolute magnitude, preferring the positive value when magnitudes are equal.
    /// </summary>
    /// <remarks>The magnitude of <see cref="MinValue"/> is treated as 2^255 without overflow.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 MaxMagnitude(in Int256 a, in Int256 b)
    {
        AsUpstream(in a).Abs(out var aMagnitude);
        AsUpstream(in b).Abs(out var bMagnitude);
        int comparison = aMagnitude._value.CompareTo(in bMagnitude._value);
        return (comparison > 0) || ((comparison == 0) && !a.IsNegative) ? a : b;
    }

    /// <summary>
    /// Adds two values, storing the wrapped two's-complement sum in <paramref name="result"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the sum overflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Add(in Int256 a, in Int256 b, out Int256 result)
    {
        bool aNegative = a.IsNegative;
        bool bNegative = b.IsNegative;
        UInt256.Add(in a._value, in b._value, out var bits);
        result = new Int256(bits);
        return (aNegative == bNegative) && (aNegative != result.IsNegative);
    }

    /// <summary>
    /// Subtracts two values, storing the wrapped two's-complement difference in <paramref name="result"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the difference overflows; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Subtract(in Int256 a, in Int256 b, out Int256 result)
    {
        bool aNegative = a.IsNegative;
        bool bNegative = b.IsNegative;
        UInt256.Subtract(in a._value, in b._value, out var bits);
        result = new Int256(bits);
        return (aNegative != bNegative) && (aNegative != result.IsNegative);
    }
}
