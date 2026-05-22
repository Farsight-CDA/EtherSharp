#pragma warning disable CS1591

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    private static readonly BigInteger _minValueAsBigInteger = -(BigInteger.One << 255);
    private static readonly BigInteger _maxValueAsBigInteger = (BigInteger.One << 255) - BigInteger.One;

    public static explicit operator Int256(in UInt256 value) => new Int256(value);

    public static implicit operator Int256(byte a) => new Int256((UInt256) a);
    public static implicit operator Int256(sbyte a) => new Int256(a);

    public static implicit operator Int256(ushort a) => new Int256((UInt256) a);
    public static implicit operator Int256(short a) => new Int256(a);

    public static implicit operator Int256(uint a) => new Int256((UInt256) a);
    public static implicit operator Int256(int a) => new Int256(a);

    public static implicit operator Int256(ulong a) => new Int256((UInt256) a);
    public static implicit operator Int256(long a) => new Int256(a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator BigInteger(in Int256 x)
    {
        if((x._value._u1 | x._value._u2 | x._value._u3) == 0 && x._value._u0 <= Int64.MaxValue)
        {
            return new BigInteger((long) x._value._u0);
        }

        if(x._value._u1 == UInt64.MaxValue && x._value._u2 == UInt64.MaxValue &&
           x._value._u3 == UInt64.MaxValue && x._value._u0 >= 0x8000000000000000UL)
        {
            return new BigInteger(unchecked((long) x._value._u0));
        }

        var bytes = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Int256, byte>(ref Unsafe.AsRef(in x)), 32);
        return new BigInteger(bytes);
    }

    public static explicit operator Int256(in BigInteger value)
    {
        if(value < _minValueAsBigInteger || value > _maxValueAsBigInteger)
        {
            throw new OverflowException($"Cannot convert BigInteger value to Int256: {value}.");
        }

        Span<byte> bytes = stackalloc byte[32];
        if(value.Sign < 0)
        {
            bytes.Fill(0xFF);
        }

        value.TryWriteBytes(bytes, out _);
        return new Int256(bytes);
    }

    public static implicit operator long(in Int256 value)
    {
        long res = unchecked((long) value._value._u0);
        return new Int256(res) != value
            ? throw new OverflowException("Cannot convert Int256 value to long.")
            : res;
    }

    public static implicit operator ulong(in Int256 value)
        => (value._value._u1 | value._value._u2 | value._value._u3) != 0
            ? throw new OverflowException("Cannot convert Int256 value to ulong.")
            : value._value._u0;

    public static implicit operator int(in Int256 value)
    {
        long res = value;
        return res < Int32.MinValue || res > Int32.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to int.")
            : (int) res;
    }

    public static implicit operator uint(in Int256 value)
    {
        ulong ul = value;
        return ul > UInt32.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to uint.")
            : (uint) ul;
    }

    public static implicit operator short(in Int256 value)
    {
        long res = value;
        return res < Int16.MinValue || res > Int16.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to short.")
            : (short) res;
    }

    public static implicit operator ushort(in Int256 value)
    {
        ulong ul = value;
        return ul > UInt16.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to ushort.")
            : (ushort) ul;
    }

    public static implicit operator sbyte(in Int256 value)
    {
        long res = value;
        return res < SByte.MinValue || res > SByte.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to sbyte.")
            : (sbyte) res;
    }

    public static implicit operator byte(in Int256 value)
    {
        ulong ul = value;
        return ul > Byte.MaxValue
            ? throw new OverflowException("Cannot convert Int256 value to byte.")
            : (byte) ul;
    }

    public static explicit operator double(in Int256 x)
        => !x.IsNegative
            ? (double) x._value
            : -(double) (-x)._value;

    public static explicit operator decimal(in Int256 x)
        => (decimal) (BigInteger) x;

    public static explicit operator Int256(decimal value)
    {
        var integer = (BigInteger) value;
        if(integer < _minValueAsBigInteger || integer > _maxValueAsBigInteger)
        {
            throw new OverflowException("Cannot convert decimal value to Int256.");
        }

        if(integer >= 0)
        {
            return new Int256((UInt256) integer);
        }

        var abs = BigInteger.Abs(integer);
        var absValue = new Int256((UInt256) abs);
        return Negate(absValue);
    }
}
