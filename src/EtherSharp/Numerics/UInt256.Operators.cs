using System.Runtime.CompilerServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Determines whether two values are equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in UInt256 a, in UInt256 b)
        => a.Equals(in b);

    /// <summary>
    /// Determines whether two values are not equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in UInt256 a, in UInt256 b)
        => !a.Equals(in b);

    /// <summary>
    /// Determines whether the first value is less than the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(in UInt256 a, in UInt256 b)
        => AsUpstream(in a) < AsUpstream(in b);

    /// <summary>
    /// Determines whether the first value is greater than the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(in UInt256 a, in UInt256 b)
        => AsUpstream(in a) > AsUpstream(in b);

    /// <summary>
    /// Determines whether the first value is less than or equal to the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(in UInt256 a, in UInt256 b)
        => AsUpstream(in a) <= AsUpstream(in b);

    /// <summary>
    /// Determines whether the first value is greater than or equal to the second value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(in UInt256 a, in UInt256 b)
        => AsUpstream(in a) >= AsUpstream(in b);

    /// <summary>
    /// Returns the value unchanged.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator +(in UInt256 value)
        => value;

    /// <summary>
    /// Negates a value, wrapping modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator -(in UInt256 value)
    {
        Negate(in value, out var result);
        return result;
    }

    /// <summary>
    /// Negates a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is not zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked -(in UInt256 value)
        => value.IsZero
            ? Zero
            : throw new OverflowException();

    /// <summary>
    /// Adds two values, wrapping modulo 2^256 on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator +(in UInt256 a, in UInt256 b)
    {
        Add(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Adds two values.
    /// </summary>
    /// <exception cref="OverflowException">The sum exceeds <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked +(in UInt256 a, in UInt256 b)
        => Add(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Subtracts two values, wrapping modulo 2^256 on underflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator -(in UInt256 a, in UInt256 b)
    {
        Subtract(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Subtracts two values.
    /// </summary>
    /// <exception cref="OverflowException">The difference is below <see cref="MinValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked -(in UInt256 a, in UInt256 b)
        => Subtract(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Multiplies two values, wrapping modulo 2^256 on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator *(in UInt256 a, in UInt256 b)
    {
        Multiply(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Multiplies two values.
    /// </summary>
    /// <exception cref="OverflowException">The product exceeds <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked *(in UInt256 a, in UInt256 b)
        => MultiplyOverflow(in a, in b, out var result)
            ? throw new OverflowException()
            : result;

    /// <summary>
    /// Divides two values, returning the integer quotient.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator /(in UInt256 a, in UInt256 b)
    {
        Divide(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Computes the remainder of dividing <paramref name="a"/> by <paramref name="b"/>.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="b"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator %(in UInt256 a, in UInt256 b)
    {
        Remainder(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Increments a value, wrapping modulo 2^256 on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator ++(in UInt256 value)
        => unchecked(value + One);

    /// <summary>
    /// Increments a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked ++(in UInt256 value)
        => checked(value + One);

    /// <summary>
    /// Decrements a value, wrapping modulo 2^256 on underflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator --(in UInt256 value)
        => unchecked(value - One);

    /// <summary>
    /// Decrements a value.
    /// </summary>
    /// <exception cref="OverflowException">The value is <see cref="MinValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator checked --(in UInt256 value)
        => checked(value - One);

    /// <summary>
    /// Computes the bitwise AND of two 256-bit values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator &(in UInt256 a, in UInt256 b)
    {
        And(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Computes the bitwise OR of two 256-bit values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator |(in UInt256 a, in UInt256 b)
    {
        Or(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Computes the bitwise exclusive OR of two 256-bit values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator ^(in UInt256 a, in UInt256 b)
    {
        Xor(in a, in b, out var result);
        return result;
    }

    /// <summary>
    /// Complements every bit of the 256-bit representation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator ~(in UInt256 value)
    {
        Not(in value, out var result);
        return result;
    }

    /// <summary>
    /// Shifts left, discarding bits shifted out of the 256-bit range.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator <<(in UInt256 a, int n)
    {
        ShiftLeft(in a, n, out var result);
        return result;
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with zero.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator >>(in UInt256 a, int n)
    {
        ShiftRightLogical(in a, n, out var result);
        return result;
    }

    /// <summary>
    /// Shifts right, filling the vacated high bits with zero.
    /// </summary>
    /// <remarks>
    /// Only the low eight bits of the shift count are used: <c>n &amp; 255</c>.
    /// Shifts behave identically in checked and unchecked contexts.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt256 operator >>>(in UInt256 a, int n)
    {
        ShiftRightLogical(in a, n, out var result);
        return result;
    }
}
