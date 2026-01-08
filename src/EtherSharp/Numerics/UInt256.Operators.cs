// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    public static explicit operator UInt256(Int256 z) => z._value;

    public static implicit operator UInt256(byte a) => new UInt256(a);
    public static explicit operator UInt256(sbyte a)
        => a < 0
            ? throw new ArgumentException($"Expected a positive number and got {a}", nameof(a))
            : new UInt256((ulong) a);

    public static implicit operator UInt256(ushort a) => new UInt256(a);
    public static explicit operator UInt256(short a)
        => a < 0
            ? throw new ArgumentException($"Expected a positive number and got {a}", nameof(a))
            : new UInt256((ulong) a);

    public static implicit operator UInt256(uint a) => new(a);
    public static explicit operator UInt256(int n)
        => n < 0
            ? throw new ArgumentException("n < 0")
            : new UInt256((ulong) n);

    public static implicit operator UInt256(ulong value) => new UInt256(value);
    public static explicit operator UInt256(long a)
        => a < 0
            ? throw new ArgumentException($"Expected a positive number and got {a}", nameof(a))
            : new UInt256((ulong) a);

    public static explicit operator UInt256(in BigInteger a)
    {
        if(a.Sign == -1)
        {
            throw new ArgumentException($"Expected a positive number and got {a}", nameof(a));
        }

        int byteCount = a.GetByteCount(true);

        if(byteCount > 32)
        {
            throw new ArgumentException($"BigInteger too large for UInt256: {a}", nameof(a));
        }

        Span<byte> buffer = stackalloc byte[32];
        a.TryWriteBytes(buffer[(32 - byteCount)..], out _, true, true);
        return new UInt256(buffer, true);
    }

    public static explicit operator BigInteger(in UInt256 value)
    {
        var bytes = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<UInt256, byte>(ref Unsafe.AsRef(in value)), 32);
        return new BigInteger(bytes, true);
    }

    public static explicit operator sbyte(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > (long) SByte.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to sbyte.")
            : (sbyte) a._u0;

    public static explicit operator byte(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > Byte.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to byte.")
            : (byte) a._u0;

    public static explicit operator short(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > (long) Int16.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to short.")
            : (short) a._u0;

    public static explicit operator ushort(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > UInt16.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to ushort.")
            : (ushort) a._u0;

    public static explicit operator int(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > Int32.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to int.")
            : (int) a._u0;

    public static explicit operator uint(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > UInt32.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to uint.")
            : (uint) a._u0;

    public static explicit operator long(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0 || a._u0 > Int64.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to long.")
            : (long) a._u0;

    public static explicit operator ulong(in UInt256 a)
        => a._u1 > 0 || a._u2 > 0 || a._u3 > 0
            ? throw new OverflowException("Cannot convert UInt256 value to ulong.")
            : a._u0;

    public static explicit operator double(in UInt256 a)
    {
        double multiplier = UInt64.MaxValue;
        return (((((a._u3 * multiplier) + a._u2) * multiplier) + a._u1) * multiplier) + a._u0;
    }

    public static explicit operator UInt256(double a)
    {
        if(a < 0)
        {
            throw new ArgumentException($"Expected a positive number and got {a}", nameof(a));
        }

        UInt256 c;

        if(a <= UInt64.MaxValue)
        {
            ulong cu0 = (ulong) a;
            ulong cu1 = 0;
            ulong cu2 = 0;
            ulong cu3 = 0;
            c = new UInt256(cu0, cu1, cu2, cu3);
        }
        else
        {
            int shift = Math.Max((int) Math.Ceiling(Math.Log(a, 2)) - 63, 0);
            ulong cu0 = (ulong) (a / Math.Pow(2, shift));
            ulong cu1 = 0;
            ulong cu2 = 0;
            ulong cu3 = 0;
            c = new UInt256(cu0, cu1, cu2, cu3);
            c <<= shift;
        }

        return c;
    }

    [OverloadResolutionPriority(1)]
    public static bool operator ==(in UInt256 a, in UInt256 b) => a.Equals(b);
    public static bool operator ==(UInt256 a, UInt256 b) => a.Equals(in b);
    public static bool operator ==(in UInt256 a, int b) => a.Equals(b);
    public static bool operator ==(int a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, long b) => a.Equals(b);
    public static bool operator ==(long a, in UInt256 b) => b.Equals(a);
    public static bool operator ==(in UInt256 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, in UInt256 b) => b.Equals(a);

    [OverloadResolutionPriority(1)]
    public static bool operator !=(in UInt256 a, in UInt256 b) => !a.Equals(b);
    public static bool operator !=(UInt256 a, UInt256 b) => !a.Equals(b);
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
    public static bool operator <=(ulong a, UInt256 b) => !LessThan(in b, a);

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

    [OverloadResolutionPriority(1)]
    public static UInt256 operator +(in UInt256 a, in UInt256 b)
    {
        Add(in a, in b, out var res);
        return res;
    }
    public static UInt256 operator +(UInt256 a, UInt256 b)
    {
        Add(in a, in b, out var res);
        return res;
    }

    [OverloadResolutionPriority(1)]
    public static UInt256 operator checked +(in UInt256 a, in UInt256 b)
        => Add(in a, in b, out var res)
            ? throw new OverflowException($"Overflow in addition {a} + {b}")
            : res;
    public static UInt256 operator checked +(UInt256 a, UInt256 b)
        => Add(in a, in b, out var res)
            ? throw new OverflowException($"Overflow in addition {a} + {b}")
            : res;

    [OverloadResolutionPriority(1)]
    public static UInt256 operator ++(in UInt256 a)
    {
        Add(in a, UInt256.One, out var res);
        return res;
    }
    public static UInt256 operator ++(UInt256 a)
    {
        Add(in a, UInt256.One, out var res);
        return res;
    }
    [OverloadResolutionPriority(1)]
    public static UInt256 operator checked ++(in UInt256 a)
        => Add(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Overflow in addition {a}++")
            : res;
    public static UInt256 operator checked ++(UInt256 a)
        => Add(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Overflow in addition {a}++")
            : res;

    [OverloadResolutionPriority(1)]
    public static UInt256 operator -(in UInt256 a, in UInt256 b)
    {
        Subtract(in a, in b, out var res);
        return res;
    }
    public static UInt256 operator -(UInt256 a, UInt256 b)
    {
        Subtract(in a, in b, out var res);
        return res;
    }
    [OverloadResolutionPriority(1)]
    public static UInt256 operator checked -(in UInt256 a, in UInt256 b)
        => Subtract(in a, in b, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - {b}")
            : res;
    public static UInt256 operator checked -(UInt256 a, UInt256 b)
        => Subtract(in a, in b, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - {b}")
            : res;

    [OverloadResolutionPriority(1)]
    public static UInt256 operator --(in UInt256 a)
    {
        Subtract(in a, UInt256.One, out var res);
        return res;
    }
    public static UInt256 operator --(UInt256 a)
    {
        Subtract(in a, UInt256.One, out var res);
        return res;
    }

    [OverloadResolutionPriority(1)]
    public static UInt256 operator checked --(in UInt256 a)
        => Subtract(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - 1")
            : res;
    public static UInt256 operator checked --(UInt256 a)
        => Subtract(in a, UInt256.One, out var res)
            ? throw new OverflowException($"Underflow in subtraction {a} - 1")
            : res;

    [OverloadResolutionPriority(1)]
    public static UInt256 operator *(in UInt256 a, in UInt256 b)
    {
        Multiply(in a, in b, out var c);
        return c;
    }
    public static UInt256 operator *(UInt256 a, UInt256 b)
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

    [OverloadResolutionPriority(1)]
    public static UInt256 operator /(in UInt256 a, in UInt256 b)
    {
        Divide(in a, in b, out var res);
        return res;
    }
    public static UInt256 operator /(UInt256 a, UInt256 b)
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

    [OverloadResolutionPriority(1)]
    public static UInt256 operator -(in UInt256 value) => Negate(in value);
    public static UInt256 operator -(UInt256 value) => Negate(in value);
    [OverloadResolutionPriority(1)]
    public static UInt256 operator +(in UInt256 value) => value;
    public static UInt256 operator +(UInt256 value) => value;
}