// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Numerics;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Calculates the floor of <c>x * y / denominator</c> with full precision.
    /// </summary>
    /// <param name="x">The first 256-bit factor.</param>
    /// <param name="y">The second 256-bit factor.</param>
    /// <param name="denominator">The divisor.</param>
    /// <returns>The floor of <c>x * y / denominator</c>.</returns>
    /// <exception cref="System.DivideByZeroException"><paramref name="denominator"/> is zero.</exception>
    /// <exception cref="System.OverflowException">The quotient does not fit in a <see cref="UInt256"/>.</exception>
    public static UInt256 MulDiv(in UInt256 x, in UInt256 y, in UInt256 denominator)
        => MulDivOverflow(in x, in y, in denominator, out var result)
            ? throw new OverflowException("The MulDiv quotient does not fit in UInt256.")
            : result;

    /// <summary>
    /// Calculates the floor of <c>x * y / denominator</c> with full precision and reports whether the quotient overflowed.
    /// </summary>
    /// <param name="x">The first 256-bit factor.</param>
    /// <param name="y">The second 256-bit factor.</param>
    /// <param name="denominator">The divisor.</param>
    /// <param name="result">
    /// On return, contains the floor of <c>x * y / denominator</c>, or zero if the quotient overflowed.
    /// </param>
    /// <returns><see langword="true"/> if the quotient does not fit in a <see cref="UInt256"/>; otherwise <see langword="false"/>.</returns>
    /// <exception cref="System.DivideByZeroException"><paramref name="denominator"/> is zero.</exception>
    public static bool MulDivOverflow(in UInt256 x, in UInt256 y, in UInt256 denominator, out UInt256 result)
    {
        if(denominator.IsZero)
        {
            throw new DivideByZeroException();
        }

        Multiply256To512Bit(in x, in y, out var productLow, out var productHigh);
        if(productHigh.IsZero)
        {
            Divide(in productLow, in denominator, out result);
            return false;
        }

        // A quotient of 2^256 or greater cannot be represented by UInt256.
        if(denominator <= productHigh)
        {
            result = default;
            return true;
        }

        int shift = denominator._u0 != 0
            ? BitOperations.TrailingZeroCount(denominator._u0)
            : denominator._u1 != 0
                ? 64 + BitOperations.TrailingZeroCount(denominator._u1)
                : denominator._u2 != 0
                    ? 128 + BitOperations.TrailingZeroCount(denominator._u2)
                    : 192 + BitOperations.TrailingZeroCount(denominator._u3);

        var oddDenominator = denominator >> shift;
        if(oddDenominator.IsOne)
        {
            result = (productLow >> shift) | (productHigh << (256 - shift));
            return false;
        }

        Remainder512By256Bits(in productLow, in productHigh, in denominator, true, out var remainder);
        if(Subtract(in productLow, in remainder, out productLow))
        {
            Subtract(in productHigh, One, out productHigh);
        }

        if(shift != 0)
        {
            productLow = (productLow >> shift) | (productHigh << (256 - shift));
        }

        var inverse = ModularInverse(in oddDenominator);
        result = productLow * inverse;
        return false;
    }

    // Solve one base-2^64 limb at a time so inverse construction only uses widening 64-bit products.
    private static UInt256 ModularInverse(in UInt256 denominator)
    {
        ulong d0 = denominator._u0;
        ulong q0 = (3 * d0) ^ 2;
        q0 *= 2 - (d0 * q0);
        q0 *= 2 - (d0 * q0);
        q0 *= 2 - (d0 * q0);
        q0 *= 2 - (d0 * q0);

        ulong carryLow = Multiply64(d0, q0, out _);
        ulong carryHigh = 0;

        ulong a0 = carryLow;
        ulong a1 = carryHigh;
        ulong a2 = 0;
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u1, q0);
        ulong q1 = (0 - a0) * q0;
        MultiplyAddCarry(ref a0, ref a1, ref a2, d0, q1);
        carryLow = a1;
        carryHigh = a2;

        a0 = carryLow;
        a1 = carryHigh;
        a2 = 0;
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u2, q0);
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u1, q1);
        ulong q2 = (0 - a0) * q0;
        MultiplyAddCarry(ref a0, ref a1, ref a2, d0, q2);
        carryLow = a1;
        carryHigh = a2;

        a0 = carryLow;
        a1 = carryHigh;
        a2 = 0;
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u3, q0);
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u2, q1);
        MultiplyAddCarry(ref a0, ref a1, ref a2, denominator._u1, q2);
        ulong q3 = (0 - a0) * q0;

        return Create(q0, q1, q2, q3);
    }
}
