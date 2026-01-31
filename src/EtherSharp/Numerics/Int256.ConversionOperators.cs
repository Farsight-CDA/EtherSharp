using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    public static explicit operator Int256(UInt256 value) => new Int256(value);

    public static implicit operator Int256(byte a) => new Int256((UInt256) a);
    public static implicit operator Int256(sbyte a) => new Int256(a);

    public static implicit operator Int256(ushort a) => new Int256((UInt256) a);
    public static implicit operator Int256(short a) => new Int256(a);

    public static implicit operator Int256(uint a) => new Int256((UInt256) a);
    public static implicit operator Int256(int a) => new Int256(a);

    public static implicit operator Int256(ulong a) => new Int256((UInt256) a);
    public static implicit operator Int256(long a) => new Int256(a);

    public static explicit operator BigInteger(Int256 x)
    {
        Span<byte> bytes = stackalloc byte[32];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], x._value._u0);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8, 8), x._value._u1);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(16, 8), x._value._u2);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(24, 8), x._value._u3);
        return new BigInteger(bytes);
    }
}
