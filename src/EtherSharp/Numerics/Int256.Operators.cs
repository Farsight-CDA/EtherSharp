using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    /// <summary>
    /// Determines whether two values are equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in Int256 a, in Int256 b)
        => a.Equals(in b);

    /// <summary>
    /// Determines whether two values are not equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in Int256 a, in Int256 b)
        => !a.Equals(in b);

    /// <summary>
    /// Determines whether the first value is less than the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(in Int256 a, in Int256 b)
        => AsUpstream(in a) < AsUpstream(in b);

    /// <summary>
    /// Determines whether the first value is greater than the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(in Int256 a, in Int256 b)
        => AsUpstream(in a) > AsUpstream(in b);

    /// <summary>
    /// Determines whether the first value is less than or equal to the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(in Int256 a, in Int256 b)
        => !(AsUpstream(in a) > AsUpstream(in b));

    /// <summary>
    /// Determines whether the first value is greater than or equal to the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(in Int256 a, in Int256 b)
        => !(AsUpstream(in a) < AsUpstream(in b));

    /// <summary>
    /// Returns the value unchanged.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator +(in Int256 value)
        => value;

    /// <summary>
    /// Negates a value, wrapping in two's-complement form on overflow.
    /// </summary>
    /// <remarks>Negating <see cref="MinValue"/> wraps to <see cref="MinValue"/>.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator -(in Int256 value)
    {
        Negate(in value, out var result);
        return result;
    }

    /// <summary>
    /// Negates a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is <see cref="MinValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked -(in Int256 value)
        => value == MinValue
            ? throw new OverflowException()
            : -value;

    /// <summary>
    /// Adds two values, wrapping in two's-complement form on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator +(in Int256 a, in Int256 b)
    {
        Add(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Adds two values.
    /// </summary>
    /// <exception cref="OverflowException">The sum is outside the signed 256-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked +(in Int256 a, in Int256 b)
        => Add(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Subtracts two values, wrapping in two's-complement form on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator -(in Int256 a, in Int256 b)
    {
        Subtract(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Subtracts two values.
    /// </summary>
    /// <exception cref="OverflowException">The difference is outside the signed 256-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked -(in Int256 a, in Int256 b)
        => Subtract(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Multiplies two values, wrapping in two's-complement form on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator *(in Int256 a, in Int256 b)
    {
        Multiply(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Multiplies two values.
    /// </summary>
    /// <exception cref="OverflowException">The product is outside the signed 256-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked *(in Int256 a, in Int256 b)
        => MultiplyOverflow(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Divides two values, returning the quotient truncated toward zero.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    /// <exception cref="OverflowException"><paramref name="a"/> is <see cref="MinValue"/> and <paramref name="b"/> is minus one, including in an unchecked context.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator /(in Int256 a, in Int256 b)
    {
        Divide(in a, in b, out var result);
        return result;
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
    public static Int256 operator %(in Int256 a, in Int256 b)
    {
        Remainder(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Increments a value, wrapping in two's-complement form on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator ++(in Int256 value)
        => unchecked(value + One);

    /// <summary>
    /// Increments a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked ++(in Int256 value)
        => value == MaxValue
            ? throw new OverflowException()
            : unchecked(value + One);

    /// <summary>
    /// Decrements a value, wrapping in two's-complement form on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator --(in Int256 value)
        => unchecked(value - One);

    /// <summary>
    /// Decrements a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is <see cref="MinValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator checked --(in Int256 value)
        => value == MinValue
            ? throw new OverflowException()
            : unchecked(value - One);

    /// <summary>
    /// Computes the bitwise AND of two 256-bit two's-complement representations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator &(in Int256 a, in Int256 b)
    {
        And(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Computes the bitwise OR of two 256-bit two's-complement representations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator |(in Int256 a, in Int256 b)
    {
        Or(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Computes the bitwise exclusive OR of two 256-bit two's-complement representations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator ^(in Int256 a, in Int256 b)
    {
        Xor(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Complements every bit of the 256-bit two's-complement representation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator ~(in Int256 value)
    {
        Not(in value, out var result);
        return result;
    }

    /// <summary>
    /// Shifts left, discarding bits shifted out of the two's-complement representation.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator <<(in Int256 a, int n)
    {
        ShiftLeft(in a, n, out var result);
        return result;
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with the sign bit.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator >>(in Int256 a, int n)
    {
        ShiftRightArithmetic(in a, n, out var result);
        return result;
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with zero regardless of the sign.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int256 operator >>>(in Int256 a, int n)
    {
        ShiftRightLogical(in a, n, out var result);
        return result;
    }
}
