#pragma warning disable CS1591

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    public static bool operator ==(in Int256 a, in Int256 b) => a.Equals(b);
    public static bool operator ==(in Int256 a, int b) => a.Equals(b);
    public static bool operator ==(int a, in Int256 b) => b.Equals(a);
    public static bool operator ==(in Int256 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, in Int256 b) => b.Equals(a);
    public static bool operator ==(in Int256 a, long b) => a.Equals(b);
    public static bool operator ==(long a, in Int256 b) => b.Equals(a);
    public static bool operator ==(in Int256 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, in Int256 b) => b.Equals(a);

    public static bool operator !=(in Int256 a, in Int256 b) => !a.Equals(b);
    public static bool operator !=(in Int256 a, int b) => !a.Equals(b);
    public static bool operator !=(int a, in Int256 b) => !b.Equals(a);
    public static bool operator !=(in Int256 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, in Int256 b) => !b.Equals(a);
    public static bool operator !=(in Int256 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, in Int256 b) => !b.Equals(a);
    public static bool operator !=(in Int256 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, in Int256 b) => !b.Equals(a);

    public static bool operator <(in Int256 a, in Int256 b) => LessThan(in a, in b);
    public static bool operator <(in Int256 a, int b) => LessThan(in a, b);
    public static bool operator <(int a, in Int256 b) => LessThan(a, in b);
    public static bool operator <(in Int256 a, uint b) => LessThan(in a, b);
    public static bool operator <(uint a, in Int256 b) => LessThan(a, in b);
    public static bool operator <(in Int256 a, long b) => LessThan(in a, b);
    public static bool operator <(long a, in Int256 b) => LessThan(a, in b);
    public static bool operator <(in Int256 a, ulong b) => LessThan(in a, b);
    public static bool operator <(ulong a, in Int256 b) => LessThan(a, in b);

    public static bool operator <=(in Int256 a, in Int256 b) => !LessThan(in b, in a);
    public static bool operator <=(in Int256 a, int b) => !LessThan(b, in a);
    public static bool operator <=(int a, in Int256 b) => !LessThan(in b, a);
    public static bool operator <=(in Int256 a, uint b) => !LessThan(b, in a);
    public static bool operator <=(uint a, in Int256 b) => !LessThan(in b, a);
    public static bool operator <=(in Int256 a, long b) => !LessThan(b, in a);
    public static bool operator <=(long a, in Int256 b) => !LessThan(in b, a);
    public static bool operator <=(in Int256 a, ulong b) => !LessThan(b, in a);
    public static bool operator <=(ulong a, Int256 b) => !LessThan(in b, a);

    public static bool operator >(in Int256 a, in Int256 b) => LessThan(in b, in a);
    public static bool operator >(in Int256 a, int b) => LessThan(b, in a);
    public static bool operator >(int a, in Int256 b) => LessThan(in b, a);
    public static bool operator >(in Int256 a, uint b) => LessThan(b, in a);
    public static bool operator >(uint a, in Int256 b) => LessThan(in b, a);
    public static bool operator >(in Int256 a, long b) => LessThan(b, in a);
    public static bool operator >(long a, in Int256 b) => LessThan(in b, a);
    public static bool operator >(in Int256 a, ulong b) => LessThan(b, in a);
    public static bool operator >(ulong a, in Int256 b) => LessThan(in b, a);

    public static bool operator >=(in Int256 a, in Int256 b) => !LessThan(in a, in b);
    public static bool operator >=(in Int256 a, int b) => !LessThan(in a, b);
    public static bool operator >=(int a, in Int256 b) => !LessThan(a, in b);
    public static bool operator >=(in Int256 a, uint b) => !LessThan(in a, b);
    public static bool operator >=(uint a, in Int256 b) => !LessThan(a, in b);
    public static bool operator >=(in Int256 a, long b) => !LessThan(in a, b);
    public static bool operator >=(long a, in Int256 b) => !LessThan(a, in b);
    public static bool operator >=(in Int256 a, ulong b) => !LessThan(in a, b);
    public static bool operator >=(ulong a, in Int256 b) => !LessThan(a, in b);

    public static Int256 operator +(in Int256 a, in Int256 b)
    {
        Add(in a, in b, out var res);
        return res;
    }

    public static Int256 operator checked +(in Int256 a, in Int256 b)
        => AddWithOverflow(in a, in b, out var res)
            ? throw new OverflowException($"Overflow in addition {a} + {b}")
            : res;

    public static Int256 operator ++(in Int256 a)
    {
        Add(in a, Int256.One, out var res);
        return res;
    }

    public static Int256 operator checked ++(in Int256 a)
        => AddWithOverflow(in a, Int256.One, out var res)
            ? throw new OverflowException($"Overflow in addition {a}++")
            : res;

    public static Int256 operator -(in Int256 a, in Int256 b)
    {
        Subtract(in a, in b, out var res);
        return res;
    }

    public static Int256 operator checked -(in Int256 a, in Int256 b)
        => SubtractWithOverflow(in a, in b, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - {b}")
            : res;

    public static Int256 operator --(in Int256 a)
    {
        Subtract(in a, Int256.One, out var res);
        return res;
    }

    public static Int256 operator checked --(in Int256 a)
        => SubtractWithOverflow(in a, Int256.One, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - 1")
            : res;

    public static Int256 operator *(in Int256 a, in Int256 b)
    {
        Multiply(in a, in b, out var c);
        return c;
    }

    public static Int256 operator *(in Int256 a, uint b)
    {
        Multiply(in a, b, out var c);
        return c;
    }
    public static Int256 operator *(uint a, in Int256 b)
    {
        Multiply(a, in b, out var c);
        return c;
    }

    public static Int256 operator *(in Int256 a, ulong b)
    {
        Multiply(in a, b, out var c);
        return c;
    }
    public static Int256 operator *(ulong a, in Int256 b)
    {
        Multiply(a, in b, out var c);
        return c;
    }

    public static Int256 operator /(in Int256 a, in Int256 b)
    {
        Divide(in a, in b, out var res);
        return res;
    }
    public static Int256 operator /(in Int256 a, uint b)
    {
        Divide(in a, b, out var c);
        return c;
    }

    public static Int256 operator <<(in Int256 a, int n)
    {
        Int256.LeftShift(a, n, out var res);
        return res;
    }

    public static Int256 operator >>(in Int256 a, int n)
    {
        Int256.RightShift(a, n, out var res);
        return res;
    }

    public static Int256 operator ^(in Int256 a, in Int256 b)
        => Xor(a, b);

    public static Int256 operator ~(in Int256 a)
        => Not(in a);

    public static Int256 operator |(in Int256 a, in Int256 b)
        => Or(a, b);

    public static Int256 operator &(in Int256 a, in Int256 b)
        => And(a, b);

    public static Int256 operator %(in Int256 a, in Int256 b)
        => Mod(a, b);

    public static Int256 operator -(in Int256 value) => Negate(in value);
    public static Int256 operator +(in Int256 value) => value;
}
