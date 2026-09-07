using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Divides the exact product of two values by <paramref name="denominator"/>, checking whether the quotient exceeds <see cref="MaxValue"/>.
    /// </summary>
    /// <param name="x">The first factor.</param>
    /// <param name="y">The second factor.</param>
    /// <param name="denominator">The divisor.</param>
    /// <param name="result">The quotient rounded down, or zero if it overflows.</param>
    /// <returns><see langword="true"/> if the quotient overflows; otherwise, <see langword="false"/>.</returns>
    /// <remarks>The product is not truncated to 256 bits before division.</remarks>
    /// <exception cref="DivideByZeroException"><paramref name="denominator"/> is zero.</exception>
    [SkipLocalsInit]
    public static bool MulDivOverflow(in UInt256 x, in UInt256 y, in UInt256 denominator, out UInt256 result)
    {
        if(denominator.IsZero)
        {
            throw new DivideByZeroException();
        }

        Multiply(in x, in y, out var productLow, out var productHigh);
        // Match the scalar limb stores from the full-width multiplication.
        if((productHigh._u0 | productHigh._u1 | productHigh._u2 | productHigh._u3) == 0)
        {
            Divide(in productLow, in denominator, out result);
            return false;
        }

        // The quotient fits exactly when the divisor exceeds the high half.
        if(denominator <= productHigh)
        {
            result = default;
            return true;
        }

        int shift = TrailingZeroCount(in denominator);
        var oddDenominator = denominator >> shift;
        if(oddDenominator.IsOne)
        {
            result = (productLow >> shift) | (productHigh << (256 - shift));
            return false;
        }

        // Make the division exact using upstream's full-precision modular multiplication.
        MultiplyMod(in x, in y, in denominator, out var remainder);
        if(Subtract(in productLow, in remainder, out productLow))
        {
            Subtract(in productHigh, One, out productHigh);
        }

        if(shift != 0)
        {
            productLow = (productLow >> shift) | (productHigh << (256 - shift));
        }

        // An odd divisor is invertible modulo 2^256. The quotient is already known to fit.
        result = productLow * ModularInverse(in oddDenominator);
        return false;
    }

    private static UInt256 ModularInverse(in UInt256 denominator)
    {
        // Newton iteration doubles the number of correct bits, starting with a four-bit seed.
        ulong inverse64 = (3 * denominator._u0) ^ 2;
        inverse64 *= 2 - (denominator._u0 * inverse64);
        inverse64 *= 2 - (denominator._u0 * inverse64);
        inverse64 *= 2 - (denominator._u0 * inverse64);
        inverse64 *= 2 - (denominator._u0 * inverse64);

        var denominator128 = ((UInt128) denominator._u1 << 64) | denominator._u0;
        UInt128 inverse128 = inverse64;
        inverse128 *= 2 - (denominator128 * inverse128);

        var inverse = new UInt256((ulong) inverse128, (ulong) (inverse128 >> 64));
        return inverse * (2 - (denominator * inverse));
    }
}
