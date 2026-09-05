// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#pragma warning disable CS1591

using EtherSharp.Common.Converters.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace EtherSharp.Numerics;

/// <summary>
/// Represents a signed 256-bit integer with two's-complement semantics.
/// </summary>
[JsonConverter(typeof(Int256HexConverter))]
public readonly partial struct Int256 : IEquatable<Int256>, IComparable, IComparable<Int256>
{
    public static Int256 Zero { get; } = (Int256) 0UL;
    public static Int256 One { get; } = (Int256) 1UL;
    public static Int256 MinValue { get; } = new Int256(UInt256.One << 255);
    public static Int256 MaxValue { get; } = new Int256((UInt256.One << 255) - 1);

    public static Int256 WAD { get; } = Pow(10, 18);
    public static Int256 RAY { get; } = Pow(10, 27);

    internal readonly UInt256 _value;

    public int Sign
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value.IsZero ? 0 : IsNegative ? -1 : 1;
    }
    public bool IsNegative
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked((long) _value._u3) < 0;
    }

    public bool IsZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value.IsZero;
    }
    public bool IsOne
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value.IsOne;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Add(in Int256 a, in Int256 b, out Int256 res)
    {
        UInt256.Add(a._value, b._value, out var ures);
        res = new Int256(ures);
    }

    internal static bool AddWithOverflow(in Int256 a, in Int256 b, out Int256 res)
    {
        UInt256.Add(a._value, b._value, out var ures);
        res = new Int256(ures);

        bool aNeg = a.IsNegative;
        bool bNeg = b.IsNegative;
        bool resNeg = res.IsNegative;

        return (aNeg == bNeg) && (aNeg != resNeg);
    }

    public static void AddMod(in Int256 x, in Int256 y, in Int256 m, out Int256 res)
    {
        var mt = m;
        if(mt.IsOne)
        {
            res = Zero;
            return;
        }

        if(m.IsNegative)
        {
            mt = Negate(m);
        }
        bool xIsNegative = x.IsNegative;
        bool yIsNegative = y.IsNegative;
        if(xIsNegative && yIsNegative)
        {
            var xNeg = Negate(x);
            var yNeg = Negate(y);
            xNeg._value.AddMod(yNeg._value, mt._value, out var ures);
            res = Negate(new Int256(ures));
        }
        else if(!xIsNegative && !yIsNegative)
        {
            x._value.AddMod(y._value, mt._value, out var ures);
            res = new Int256(ures);
        }
        else
        {
            res = (x + y) % mt;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Subtract(in Int256 a, in Int256 b, out Int256 res)
    {
        UInt256.Subtract(a._value, b._value, out var ures);
        res = new Int256(ures);
    }

    internal static bool SubtractWithOverflow(in Int256 a, in Int256 b, out Int256 res)
    {
        UInt256.Subtract(a._value, b._value, out var ures);
        res = new Int256(ures);

        bool aNeg = a.IsNegative;
        bool bNeg = b.IsNegative;
        bool resNeg = res.IsNegative;

        return (aNeg != bNeg) && (aNeg != resNeg);
    }

    /// <summary>
    /// Returns the absolute difference between two <see cref="Int256"/> values.
    /// </summary>
    public static UInt256 AbsDiff(in Int256 a, in Int256 b)
    {
        if(LessThan(in a, in b))
        {
            UInt256.Subtract(in b._value, in a._value, out var result);
            return result;
        }

        UInt256.Subtract(in a._value, in b._value, out var difference);
        return difference;
    }

    public static void SubtractMod(in Int256 x, in Int256 y, in Int256 m, out Int256 res)
    {
        var mt = m;
        if(mt.IsOne)
        {
            res = Zero;
            return;
        }

        if(m.IsNegative)
        {
            mt = Negate(m);
        }
        bool xIsNegative = x.IsNegative;
        bool yIsNegative = y.IsNegative;
        if(xIsNegative && !yIsNegative)
        {
            var xNeg = Negate(x);
            xNeg._value.AddMod(y._value, mt._value, out var ures);
            res = Negate(new Int256(ures));
        }
        else if(!xIsNegative && yIsNegative)
        {
            var yNeg = Negate(y);
            x._value.AddMod(yNeg._value, mt._value, out var ures);
            res = new Int256(ures);
        }
        else
        {
            res = (x - y) % mt;
        }
    }

    internal static void Multiply(in Int256 a, in Int256 b, out Int256 res)
    {
        // Truncated multiplication is sign-agnostic in two's complement, modulo 2**256.
        Unsafe.SkipInit(out res);
        UInt256.Multiply(in a._value, in b._value, out Unsafe.As<Int256, UInt256>(ref res));
    }

    public static bool MultiplyOverflow(in Int256 x, in Int256 y, out Int256 res)
    {
        bool xIsNegative = x.IsNegative;
        bool yIsNegative = y.IsNegative;
        var xAbs = xIsNegative ? Negate(x) : x;
        var yAbs = yIsNegative ? Negate(y) : y;

        UInt256.Multiply(in xAbs._value, in yAbs._value, out var low, out var high);

        bool isNegative = xIsNegative != yIsNegative;
        res = new Int256(low);
        if(isNegative)
        {
            res = Negate(res);
        }

        var limit = isNegative ? MinValue._value : MaxValue._value;
        return !high.IsZero || limit < low;
    }

    public static void MultiplyMod(in Int256 x, in Int256 y, in Int256 m, out Int256 res)
    {
        var mAbs = m;
        if(m.IsNegative)
        {
            mAbs = Negate(m);
        }
        bool xIsNegative = x.IsNegative;
        bool yIsNegative = y.IsNegative;
        if(xIsNegative != yIsNegative)
        {
            var xAbs = x;
            var yAbs = y;
            if(xIsNegative)
            {
                xAbs = Negate(x);
            }
            else
            {
                yAbs = Negate(y);
            }
            xAbs._value.MultiplyMod(yAbs._value, mAbs._value, out var ures);
            res = new Int256(ures);
            res = Negate(res);
        }
        else
        {
            var xAbs = x;
            var yAbs = y;
            if(xIsNegative)
            {
                xAbs = Negate(x);
                yAbs = Negate(y);
            }
            xAbs._value.MultiplyMod(yAbs._value, mAbs._value, out var ures);
            res = new Int256(ures);
        }
    }

    internal static void Divide(in Int256 n, in Int256 d, out Int256 res)
    {
        bool nIsNegative = n.IsNegative;
        bool dIsNegative = d.IsNegative;
        UInt256 value;
        if(!nIsNegative)
        {
            if(!dIsNegative)
            {
                // pos / pos
                UInt256.Divide(n._value, d._value, out value);
                res = new Int256(value);
                return;
            }
            else
            {
                // pos / neg
                var neg = Negate(d);
                UInt256.Divide(n._value, neg._value, out value);
                res = new Int256(value);
                res = Negate(res);
                return;
            }
        }

        var nNeg = Negate(n);
        if(dIsNegative)
        {
            // neg / neg
            var dNeg = Negate(d);
            UInt256.Divide(nNeg._value, dNeg._value, out value);
            res = new Int256(value);
            return;
        }
        // neg / pos
        UInt256.Divide(nNeg._value, d._value, out value);
        res = new Int256(value);
        res = Negate(res);
    }

    public static Int256 Pow(in Int256 b, in Int256 e)
    {
        if(e.IsNegative)
        {
            throw new ArgumentException("exponent must be non-negative");
        }
        // Repeated raw multiplication also gives the signed power modulo 2**256.
        return new Int256(UInt256.Pow(in b._value, in e._value));
    }

    public static void ExpMod(in Int256 bs, in Int256 exp, in Int256 m, out Int256 res)
    {
        if(exp.IsNegative)
        {
            throw new ArgumentException("exponent must not be negative");
        }
        var bv = bs;
        bool switchSign = false;
        if(bs.IsNegative)
        {
            bv = Negate(bv);
            switchSign = exp._value.Bit(0);
        }
        var mAbs = m;
        if(mAbs.IsNegative)
        {
            mAbs = Negate(mAbs);
        }
        UInt256.ExpMod(bv._value, exp._value, mAbs._value, out var ures);
        res = new Int256(ures);
        if(switchSign)
        {
            res = Negate(res);
        }
    }

    // Mod returns (sign x) * { abs(x) modulus abs(y) }.
    // If y is zero, UInt256.Mod throws DivideByZeroException.
    public static Int256 Mod(in Int256 x, in Int256 y)
    {
        Int256 xIn = x, yIn = y;
        bool xIsNegative = x.IsNegative;

        // abs x
        if(xIsNegative)
        {
            xIn = Negate(x);
        }
        // abs y
        if(y.IsNegative)
        {
            yIn = Negate(y);
        }
        UInt256.Mod(in xIn._value, in yIn._value, out var value);
        var res = new Int256(value);
        return xIsNegative
            ? Negate(res)
            : res;
    }

    // Abs sets res to the absolute value
    //   Abs(0)        = 0
    //   Abs(1)        = 1
    //   Abs(2**255)   = -2**255
    //   Abs(2**256-1) = -1
    public static Int256 Abs(in Int256 value)
        => !value.IsNegative
            ? value
            : -value;

    // Neg returns -x mod 2**256.
    public static Int256 Negate(in Int256 x)
    {
        UInt256.Subtract(UInt256.Zero, x._value, out var value);
        return new Int256(value);
    }

    internal static void LeftShift(in Int256 x, int n, out Int256 res)
    {
        var ures = x._value << n;
        res = new Int256(ures);
    }

    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in Int256 other)
        => _value.Equals(other._value);
    public bool Equals(Int256 other)
        => _value.Equals(other._value);
    public override bool Equals(object? obj)
        => obj is Int256 other && Equals(other);

    public override int GetHashCode()
        => _value.GetHashCode();

    public int CompareTo(object? obj)
        => obj is not Int256 int256
            ? throw new InvalidOperationException()
            : CompareTo(int256);

    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(in Int256 b)
    {
        long top = unchecked((long) _value._u3);
        long bTop = unchecked((long) b._value._u3);
        return top != bTop
            ? top < bTop ? -1 : 1
            : _value._u2 != b._value._u2
            ? _value._u2 < b._value._u2 ? -1 : 1
            : _value._u1 != b._value._u1
            ? _value._u1 < b._value._u1 ? -1 : 1
            : _value._u0 == b._value._u0 ? 0 : _value._u0 < b._value._u0 ? -1 : 1;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Int256 b)
        => CompareTo(in b);

    internal static Int256 And(in Int256 a, in Int256 b)
    {
        UInt256.And(in a._value, in b._value, out var o);
        return new Int256(o);
    }

    internal static Int256 Xor(in Int256 a, in Int256 b)
    {
        UInt256.Xor(in a._value, in b._value, out var o);
        return new Int256(o);
    }

    internal static Int256 Or(in Int256 a, in Int256 b)
    {
        UInt256.Or(in a._value, in b._value, out var o);
        return new Int256(o);
    }

    internal static Int256 Not(in Int256 a)
    {
        UInt256.Not(in a._value, out var o);
        return new Int256(o);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool LessThan(in Int256 a, in Int256 b)
    {
        // Compare the top limb signed and all remaining limbs unsigned.
        long top = unchecked((long) a._value._u3);
        long bTop = unchecked((long) b._value._u3);
        return top != bTop
            ? top < bTop
            : a._value._u2 != b._value._u2
            ? a._value._u2 < b._value._u2
            : a._value._u1 != b._value._u1 ? a._value._u1 < b._value._u1 : a._value._u0 < b._value._u0;
    }
}
