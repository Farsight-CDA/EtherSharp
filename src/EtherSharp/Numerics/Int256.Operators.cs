using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    public static explicit operator Int256(UInt256 value) => new Int256(value);
    public static explicit operator Int256(ulong value) => new((UInt256) value);
    public static implicit operator Int256(int value) => new Int256(value);

    public static explicit operator BigInteger(Int256 x)
    {
        Span<byte> bytes = stackalloc byte[32];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], x._value._u0);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8, 8), x._value._u1);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(16, 8), x._value._u2);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(24, 8), x._value._u3);
        return new BigInteger(bytes);
    }

    public static Int256 operator -(in Int256 x)
        => Int256.Negate(x);

    public static Int256 operator %(in Int256 a, in Int256 b)
        => Int256.Mod(a, b);

    public static Int256 operator +(in Int256 a, in Int256 b)
    {
        Add(in a, in b, out var res);
        return res;
    }

    public static Int256 operator -(in Int256 a, in Int256 b)
    {
        Subtract(in a, in b, out var res);
        return res;
    }

    public static bool operator ==(in Int256 a, in Int256 b) => a.Equals(b);

    public static bool operator !=(in Int256 a, in Int256 b) => !(a == b);

    public static bool operator <(in Int256 z, in Int256 x)
    {
        int zSign = z.Sign;
        int xSign = x.Sign;

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

        return z._value < x._value;
    }
    public static bool operator >(in Int256 z, in Int256 x) => x < z;

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
}
