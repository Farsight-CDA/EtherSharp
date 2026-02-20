// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256 : IEquatable<Int256>, IComparable, IComparable<Int256>
{
    public static readonly Int256 Zero = (Int256) 0UL;
    public static readonly Int256 One = (Int256) 1UL;
    public static readonly Int256 MinusOne = new Int256(-1);
    public static readonly Int256 MinValue = new Int256(UInt256.One << 255);
    public static readonly Int256 MaxValue = new Int256((UInt256.One << 255) - 1);

    internal readonly UInt256 _value;

    public int Sign => _value.IsZero ? 0 : _value._u3 < 0x8000000000000000ul ? 1 : -1;
    public bool IsNegative => Sign < 0;

    public bool IsZero => this == Zero;
    public bool IsOne => this == One;

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

        if(m.Sign < 0)
        {
            mt = Negate(m);
        }
        int xSign = x.Sign;
        int ySign = y.Sign;
        if(xSign < 0 && ySign < 0)
        {
            var xNeg = Negate(x);
            var yNeg = Negate(y);
            xNeg._value.AddMod(yNeg._value, mt._value, out var ures);
            res = Negate(new Int256(ures));
        }
        else if(xSign > 0 && ySign > 0)
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

    public static void SubtractMod(in Int256 x, in Int256 y, in Int256 m, out Int256 res)
    {
        var mt = m;
        if(mt.IsOne)
        {
            res = Int256.Zero;
            return;
        }

        if(m.Sign < 0)
        {
            mt = Negate(m);
        }
        int xSign = x.Sign;
        int ySign = y.Sign;
        if(xSign < 0 && ySign > 0)
        {
            var xNeg = Negate(x);
            xNeg._value.AddMod(y._value, mt._value, out var ures);
            res = Negate(new Int256(ures));
        }
        else if(xSign > 0 && ySign < 0)
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
        Int256 av = a, bv = b;
        int aSign = a.Sign;
        int bSign = b.Sign;
        if(aSign < 0)
        {
            av = Negate(a);
        }
        if(bSign < 0)
        {
            bv = Negate(b);
        }
        UInt256.Multiply(av._value, bv._value, out var ures);
        res = new Int256(ures);
        if((aSign < 0 && bSign < 0) || (aSign >= 0 && bSign >= 0))
        {
            return;
        }

        res = Negate(res);
    }

    public static void MultiplyMod(in Int256 x, in Int256 y, in Int256 m, out Int256 res)
    {
        var mAbs = m;
        if(m.Sign < 0)
        {
            mAbs = Negate(m);
        }
        int xSign = x.Sign;
        int ySign = y.Sign;
        if((xSign < 0 && ySign >= 0) || (xSign >= 0 && ySign < 0))
        {
            var xAbs = x;
            var yAbs = y;
            if(xSign < 0)
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
            if(xSign < 0)
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
        UInt256 value;
        if(n.Sign >= 0)
        {
            if(d.Sign >= 0)
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
        if(d.Sign < 0)
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
        if(e.Sign < 0)
        {
            throw new ArgumentException("exponent must be non-negative");
        }
        if(b.Sign < 0)
        {
            var neg = Negate(b);
            var ures = UInt256.Pow(neg._value, e._value);

            return !e._value.Bit(0)
                ? new Int256(ures)
                : Negate(new Int256(ures));
        }
        else
        {
            var ures = UInt256.Pow(b._value, e._value);
            return new Int256(ures);
        }
    }

    public static void ExpMod(in Int256 bs, in Int256 exp, in Int256 m, out Int256 res)
    {
        if(exp < Zero)
        {
            throw new ArgumentException("exponent must not be negative");
        }
        var bv = bs;
        bool switchSign = false;
        if(bs.Sign < 0)
        {
            bv = Negate(bv);
            switchSign = exp._value.Bit(0);
        }
        var mAbs = m;
        if(mAbs.Sign < 0)
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

    // Mod sets res to (sign x) * { abs(x) modulus abs(y) }
    // If y == 0, z is set to 0 (OBS: differs from the big.Int)
    public static Int256 Mod(in Int256 x, in Int256 y)
    {
        Int256 xIn = x, yIn = y;
        int xs = x.Sign;

        // abs x
        if(xs == -1)
        {
            xIn = Negate(x);
        }
        // abs y
        if(y.Sign == -1)
        {
            yIn = Negate(y);
        }
        UInt256.Mod(in xIn._value, in yIn._value, out var value);
        var res = new Int256(value);
        return xs == -1
            ? Negate(res)
            : res;
    }

    // Abs sets res to the absolute value
    //   Abs(0)        = 0
    //   Abs(1)        = 1
    //   Abs(2**255)   = -2**255
    //   Abs(2**256-1) = -1
    public static Int256 Abs(Int256 value)
        => value.Sign >= 0
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

    private void Srsh64(out Int256 res)
        => res = new Int256(new UInt256(_value._u1, _value._u2, _value._u3, UInt64.MaxValue));

    private void Srsh128(out Int256 res)
        => res = new Int256(new UInt256(_value._u2, _value._u3, UInt64.MaxValue, UInt64.MaxValue));

    private void Srsh192(out Int256 res)
        => res = new Int256(new UInt256(_value._u3, UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue));

    internal static void RightShift(in Int256 x, int n, out Int256 res)
    {
        if(x.Sign >= 0)
        {
            var ures = x._value >> n;
            res = new Int256(ures);
            return;
        }
        if(n % 64 == 0)
        {
            switch(n)
            {
                case 0:
                    res = x;
                    return;
                case 64:
                    x.Srsh64(out res);
                    return;
                case 128:
                    x.Srsh128(out res);
                    return;
                case 192:
                    x.Srsh192(out res);
                    return;
                default:
                    res = Negate(One);
                    return;
            }
        }

        ulong z0, z1, z2, z3;
        ulong a = UInt256.Lsh(UInt64.MaxValue, 64 - (n % 64));
        // Big swaps first
        if(n > 192)
        {
            if(n > 256)
            {
                res = Negate(One);
                return;
            }
            x.Srsh192(out res);
            z1 = res._value._u1;
            z2 = res._value._u2;
            z3 = res._value._u3;
            n -= 192;
            goto sh192;
        }
        else if(n > 128)
        {
            x.Srsh128(out res);
            z2 = res._value._u2;
            z3 = res._value._u3;
            n -= 128;
            goto sh128;
        }
        else if(n > 64)
        {
            x.Srsh64(out res);
            z3 = res._value._u3;
            n -= 64;
            goto sh64;
        }
        else
        {
            res = x;
        }

        // remaining shifts
        z3 = UInt256.Rsh(res._value._u3, n) | a;
        a = UInt256.Lsh(res._value._u3, 64 - n);

    sh64:
        z2 = UInt256.Rsh(res._value._u2, n) | a;
        a = UInt256.Lsh(res._value._u2, 64 - n);

    sh128:
        z1 = UInt256.Rsh(res._value._u1, n) | a;
        a = UInt256.Lsh(res._value._u1, 64 - n);

    sh192:
        z0 = UInt256.Rsh(res._value._u0, n) | a;

        res = new Int256(new UInt256(z0, z1, z2, z3));
    }


    [OverloadResolutionPriority(1)]
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
    public int CompareTo(in Int256 b)
        => this < b ? -1 : Equals(b) ? 0 : 1;
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

    internal static bool LessThan(in Int256 a, in Int256 b)
    {
        int zSign = a.Sign;
        int xSign = b.Sign;

        if(zSign >= 0)
        {
            if(xSign < 0)
            {
                return false;
            }
        }
        else if(xSign >= 0)
        {
            return true;
        }

        return a._value < b._value;
    }
}