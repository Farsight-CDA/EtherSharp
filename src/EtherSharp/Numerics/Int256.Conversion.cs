using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256 : IStackValue<Int256>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Int256 IStackValue<Int256>.FromStackWord(in Bytes32 value)
        => (Int256) value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Bytes32 IStackValue<Int256>.ToStackWord(in Int256 value)
        => (Bytes32) value;

    /// <summary>
    /// Converts a <see cref="Byte"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(byte value)
        => (ulong) value;

    /// <summary>
    /// Converts an <see cref="SByte"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(sbyte value)
        => (long) value;

    /// <summary>
    /// Converts a <see cref="UInt16"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(ushort value)
        => (ulong) value;

    /// <summary>
    /// Converts an <see cref="Int16"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(short value)
        => (long) value;

    /// <summary>
    /// Converts a <see cref="UInt32"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(uint value)
        => (ulong) value;

    /// <summary>
    /// Converts an <see cref="Int32"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(int value)
        => (long) value;

    /// <summary>
    /// Converts a <see cref="UInt64"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(ulong value)
        => new Int256(new UInt256(value));

    /// <summary>
    /// Converts an <see cref="Int64"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(long value)
    {
        ulong signExtension = value < 0 ? UInt64.MaxValue : 0;
        return new Int256(new UInt256(
            unchecked((ulong) value), signExtension, signExtension, signExtension
        ));
    }

    /// <summary>
    /// Converts a <see cref="UInt128"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(UInt128 value)
        => new Int256((UInt256) value);

    /// <summary>
    /// Converts an <see cref="Int128"/> to an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Int256(Int128 value)
    {
        var bits = unchecked((UInt128) value);
        ulong signExtension = value < 0 ? UInt64.MaxValue : 0;
        return new Int256(new UInt256(
            unchecked((ulong) bits), (ulong) (bits >> 64), signExtension, signExtension
        ));
    }

    /// <summary>
    /// Interprets big-endian bytes as a signed 256-bit two's-complement integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Int256(in Bytes32 value)
        => BinaryPrimitives.ReadInt256BigEndian(value.DangerousGetReadOnlySpan());

    /// <summary>
    /// Reinterprets the bits of a <see cref="UInt256"/> as an <see cref="Int256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Int256(in UInt256 value)
        => new Int256(value);

    /// <summary>
    /// Converts a <see cref="UInt256"/> to an <see cref="Int256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds 2^255 - 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked Int256(in UInt256 value)
        => value._u3 > Int64.MaxValue
            ? throw new OverflowException("Cannot convert UInt256 value to Int256.")
            : unchecked((Int256) value);

    /// <summary>
    /// Converts a <see cref="BigInteger"/> to an <see cref="Int256"/>.
    /// Checks for overflow even in an unchecked context.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside [-2^255, 2^255 - 1].</exception>
    public static explicit operator Int256(in BigInteger value)
    {
        Span<byte> bytes = stackalloc byte[32];
        bytes.Fill(value.Sign < 0 ? Byte.MaxValue : (byte) 0);
        return !value.TryWriteBytes(bytes, out _, isUnsigned: false, isBigEndian: false)
            ? throw new OverflowException("Cannot convert BigInteger value to Int256.")
            : BinaryPrimitives.ReadInt256LittleEndian(bytes);
    }

    /// <summary>
    /// Converts a <see cref="Decimal"/> to an <see cref="Int256"/>, truncating toward zero.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Int256(decimal value)
        => (Int128) value;

    /// <summary>
    /// Converts to an <see cref="SByte"/>, retaining the low 8 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator sbyte(in Int256 value)
        => unchecked((sbyte) value._value._u0);

    /// <summary>
    /// Converts to an <see cref="SByte"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the signed 8-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked sbyte(in Int256 value)
        => checked((sbyte) (long) value);

    /// <summary>
    /// Converts to a <see cref="Byte"/>, retaining the low 8 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator byte(in Int256 value)
        => unchecked((byte) value._value._u0);

    /// <summary>
    /// Converts to a <see cref="Byte"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the unsigned 8-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked byte(in Int256 value)
        => checked((byte) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int16"/>, retaining the low 16 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator short(in Int256 value)
        => unchecked((short) value._value._u0);

    /// <summary>
    /// Converts to an <see cref="Int16"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the signed 16-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked short(in Int256 value)
        => checked((short) (long) value);

    /// <summary>
    /// Converts to a <see cref="UInt16"/>, retaining the low 16 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ushort(in Int256 value)
        => unchecked((ushort) value._value._u0);

    /// <summary>
    /// Converts to a <see cref="UInt16"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the unsigned 16-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked ushort(in Int256 value)
        => checked((ushort) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int32"/>, retaining the low 32 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator int(in Int256 value)
        => unchecked((int) value._value._u0);

    /// <summary>
    /// Converts to an <see cref="Int32"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the signed 32-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked int(in Int256 value)
        => checked((int) (long) value);

    /// <summary>
    /// Converts to a <see cref="UInt32"/>, retaining the low 32 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(in Int256 value)
        => unchecked((uint) value._value._u0);

    /// <summary>
    /// Converts to a <see cref="UInt32"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the unsigned 32-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked uint(in Int256 value)
        => checked((uint) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int64"/>, retaining the low 64 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator long(in Int256 value)
        => unchecked((long) value._value._u0);

    /// <summary>
    /// Converts to an <see cref="Int64"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the signed 64-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked long(in Int256 value)
    {
        ref readonly var bits = ref value._value;
        long result = unchecked((long) bits._u0);
        ulong signExtension = result < 0 ? UInt64.MaxValue : 0;
        return bits._u1 != signExtension || bits._u2 != signExtension || bits._u3 != signExtension
            ? throw new OverflowException("Cannot convert Int256 value to Int64.")
            : result;
    }

    /// <summary>
    /// Converts to a <see cref="UInt64"/>, retaining the low 64 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ulong(in Int256 value)
        => value._value._u0;

    /// <summary>
    /// Converts to a <see cref="UInt64"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the unsigned 64-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked ulong(in Int256 value)
    {
        ref readonly var bits = ref value._value;
        return (bits._u1 | bits._u2 | bits._u3) != 0
            ? throw new OverflowException("Cannot convert Int256 value to UInt64.")
            : bits._u0;
    }

    /// <summary>
    /// Converts to an <see cref="Int128"/>, retaining the low 128 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Int128(in Int256 value)
        => unchecked((Int128) (UInt128) value);

    /// <summary>
    /// Converts to an <see cref="Int128"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the signed 128-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked Int128(in Int256 value)
    {
        var result = unchecked((Int128) value);
        ulong signExtension = result < 0 ? UInt64.MaxValue : 0;
        return value._value._u2 != signExtension || value._value._u3 != signExtension
            ? throw new OverflowException("Cannot convert Int256 value to Int128.")
            : result;
    }

    /// <summary>
    /// Converts to a <see cref="UInt128"/>, retaining the low 128 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt128(in Int256 value)
        => unchecked((UInt128) value._value);

    /// <summary>
    /// Converts to a <see cref="UInt128"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the unsigned 128-bit range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt128(in Int256 value)
        => (value._value._u2 | value._value._u3) != 0
            ? throw new OverflowException("Cannot convert Int256 value to UInt128.")
            : unchecked((UInt128) value);

    /// <summary>
    /// Converts to a <see cref="BigInteger"/>.
    /// </summary>
    public static explicit operator BigInteger(in Int256 value)
    {
        ref readonly var bits = ref value._value;
        long low = unchecked((long) bits._u0);
        ulong signExtension = low < 0 ? UInt64.MaxValue : 0;
        if(bits._u1 == signExtension && bits._u2 == signExtension && bits._u3 == signExtension)
        {
            return new BigInteger(low);
        }

        Span<byte> bytes = stackalloc byte[32];
        BinaryPrimitives.WriteInt256LittleEndian(bytes, in value);
        return new BigInteger(bytes, isUnsigned: false, isBigEndian: false);
    }

    /// <summary>
    /// Converts to a <see cref="Double"/>, rounding to the nearest value with ties rounded to even.
    /// </summary>
    public static explicit operator double(in Int256 value)
    {
        if(!value.IsNegative)
        {
            return (double) value._value;
        }

        ref readonly var bits = ref value._value;
        ulong u0 = unchecked(0UL - bits._u0);
        ulong u1 = unchecked(~bits._u1 + (u0 == 0 ? 1UL : 0));
        ulong u2 = unchecked(~bits._u2 + ((u0 | u1) == 0 ? 1UL : 0));
        ulong u3 = unchecked(~bits._u3 + ((u0 | u1 | u2) == 0 ? 1UL : 0));
        return -(double) new UInt256(u0, u1, u2, u3);
    }

    /// <summary>
    /// Converts to a <see cref="Decimal"/>.
    /// Checks for overflow even in an unchecked context.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside the decimal range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator decimal(in Int256 value)
        => checked((decimal) (Int128) value);

    /// <summary>
    /// Represents the value as big-endian, 256-bit two's-complement bytes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Bytes32(in Int256 value)
        => (Bytes32) value._value;
}
