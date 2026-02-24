// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#pragma warning disable CS1591

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    public static bool operator ==(in UInt256 a, in UInt256 b) => a.Equals(b);
    public static bool operator ==(in UInt256 a, int b) => a.Equals(b);
    public static bool operator ==(int a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, long b) => a.Equals(b);
    public static bool operator ==(long a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, in UInt256 b) => b.Equals(a);

    public static bool operator !=(in UInt256 a, in UInt256 b) => !a.Equals(b);
    public static bool operator !=(in UInt256 a, int b) => !a.Equals(b);
    public static bool operator !=(int a, in UInt256 b) => !b.Equals(a);
    public static bool operator !=(in UInt256 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, in UInt256 b) => !b.Equals(a);
    public static bool operator !=(in UInt256 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, in UInt256 b) => !b.Equals(a);
    public static bool operator !=(in UInt256 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, in UInt256 b) => !b.Equals(a);

    public static bool operator <(in UInt256 a, in UInt256 b) => LessThan(in a, in b);
    public static bool operator <(in UInt256 a, int b) => LessThan(in a, b);
    public static bool operator <(int a, in UInt256 b) => LessThan(a, in b);
    public static bool operator <(in UInt256 a, uint b) => LessThan(in a, b);
    public static bool operator <(uint a, in UInt256 b) => LessThan(a, in b);
    public static bool operator <(in UInt256 a, long b) => LessThan(in a, b);
    public static bool operator <(long a, in UInt256 b) => LessThan(a, in b);
    public static bool operator <(in UInt256 a, ulong b) => LessThan(in a, b);
    public static bool operator <(ulong a, in UInt256 b) => LessThan(a, in b);

    public static bool operator <=(in UInt256 a, in UInt256 b) => !LessThan(in b, in a);
    public static bool operator <=(in UInt256 a, int b) => !LessThan(b, in a);
    public static bool operator <=(int a, in UInt256 b) => !LessThan(in b, a);
    public static bool operator <=(in UInt256 a, uint b) => !LessThan(b, in a);
    public static bool operator <=(uint a, in UInt256 b) => !LessThan(in b, a);
    public static bool operator <=(in UInt256 a, long b) => !LessThan(b, in a);
    public static bool operator <=(long a, in UInt256 b) => !LessThan(in b, a);
    public static bool operator <=(in UInt256 a, ulong b) => !LessThan(b, in a);
    public static bool operator <=(ulong a, in UInt256 b) => !LessThan(in b, a);

    public static bool operator >(in UInt256 a, in UInt256 b) => LessThan(in b, in a);
    public static bool operator >(in UInt256 a, int b) => LessThan(b, in a);
    public static bool operator >(int a, in UInt256 b) => LessThan(in b, a);
    public static bool operator >(in UInt256 a, uint b) => LessThan(b, in a);
    public static bool operator >(uint a, in UInt256 b) => LessThan(in b, a);
    public static bool operator >(in UInt256 a, long b) => LessThan(b, in a);
    public static bool operator >(long a, in UInt256 b) => LessThan(in b, a);
    public static bool operator >(in UInt256 a, ulong b) => LessThan(b, in a);
    public static bool operator >(ulong a, in UInt256 b) => LessThan(in b, a);

    public static bool operator >=(in UInt256 a, in UInt256 b) => !LessThan(in a, in b);
    public static bool operator >=(in UInt256 a, int b) => !LessThan(in a, b);
    public static bool operator >=(int a, in UInt256 b) => !LessThan(a, in b);
    public static bool operator >=(in UInt256 a, uint b) => !LessThan(in a, b);
    public static bool operator >=(uint a, in UInt256 b) => !LessThan(a, in b);
    public static bool operator >=(in UInt256 a, long b) => !LessThan(in a, b);
    public static bool operator >=(long a, in UInt256 b) => !LessThan(a, in b);
    public static bool operator >=(in UInt256 a, ulong b) => !LessThan(in a, b);
    public static bool operator >=(ulong a, in UInt256 b) => !LessThan(a, in b);

    public static UInt256 operator +(in UInt256 a, in UInt256 b)
    {
        Add(in a, in b, out var res);
        return res;
    }

    public static UInt256 operator checked +(in UInt256 a, in UInt256 b)
        => Add(in a, in b, out var res)
            ? throw new OverflowException($"Overflow in addition {a} + {b}")
            : res;

    public static UInt256 operator ++(in UInt256 a)
    {
        Add(in a, UInt256.One, out var res);
        return res;
    }

    public static UInt256 operator checked ++(in UInt256 a)
        => Add(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Overflow in addition {a}++")
            : res;

    public static UInt256 operator -(in UInt256 a, in UInt256 b)
    {
        Subtract(in a, in b, out var res);
        return res;
    }

    public static UInt256 operator checked -(in UInt256 a, in UInt256 b)
        => Subtract(in a, in b, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - {b}")
            : res;

    public static UInt256 operator --(in UInt256 a)
    {
        Subtract(in a, UInt256.One, out var res);
        return res;
    }

    public static UInt256 operator checked --(in UInt256 a)
        => Subtract(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - 1")
            : res;

    public static UInt256 operator *(in UInt256 a, in UInt256 b)
    {
        Multiply(in a, in b, out var c);
        return c;
    }

    public static UInt256 operator *(in UInt256 a, uint b)
    {
        Multiply(in a, b, out var c);
        return c;
    }
    public static UInt256 operator *(uint a, in UInt256 b)
    {
        Multiply(a, in b, out var c);
        return c;
    }

    public static UInt256 operator *(in UInt256 a, ulong b)
    {
        Multiply(in a, b, out var c);
        return c;
    }
    public static UInt256 operator *(ulong a, in UInt256 b)
    {
        Multiply(a, in b, out var c);
        return c;
    }

    public static UInt256 operator /(in UInt256 a, in UInt256 b)
    {
        Divide(in a, in b, out var res);
        return res;
    }
    public static UInt256 operator /(in UInt256 a, uint b)
    {
        Divide(in a, b, out var c);
        return c;
    }

    public static UInt256 operator <<(in UInt256 a, int n)
    {
        UInt256.LeftShift(a, n, out var res);
        return res;
    }

    public static UInt256 operator >>(in UInt256 a, int n)
    {
        UInt256.RightShift(a, n, out var res);
        return res;
    }

    public static UInt256 operator ^(in UInt256 a, in UInt256 b)
    {
        Xor(a, b, out var res);
        return res;
    }

    public static UInt256 operator ~(in UInt256 a)
    {
        Not(in a, out var res);
        return res;
    }

    public static UInt256 operator |(in UInt256 a, in UInt256 b)
    {
        Or(a, b, out var res);
        return res;
    }

    public static UInt256 operator &(in UInt256 a, in UInt256 b)
    {
        And(a, b, out var res);
        return res;
    }

    public static UInt256 operator %(in UInt256 a, in UInt256 b)
    {
        Mod(a, b, out var res);
        return res;
    }

    public static UInt256 operator -(in UInt256 value) => Negate(in value);
    public static UInt256 operator +(in UInt256 value) => value;
}
