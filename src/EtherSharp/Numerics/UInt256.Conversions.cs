using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256 : IStackValue<UInt256>
{
    private const double DOUBLE_UPPER_BOUND = 1.157920892373162E77;
    private const double DOUBLE_UINT64_UPPER_BOUND = 18446744073709551616.0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static UInt256 IStackValue<UInt256>.FromStackWord(in Bytes32 value)
        => (UInt256) value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Bytes32 IStackValue<UInt256>.ToStackWord(in UInt256 value)
        => (Bytes32) value;

    /// <summary>
    /// Converts a <see cref="Byte"/> to a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt256(byte value)
        => new UInt256(value);

    /// <summary>
    /// Converts a <see cref="UInt16"/> to a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt256(ushort value)
        => new UInt256(value);

    /// <summary>
    /// Converts a <see cref="UInt32"/> to a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt256(uint value)
        => new UInt256(value);

    /// <summary>
    /// Converts a <see cref="UInt64"/> to a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt256(ulong value)
        => new UInt256(value);

    /// <summary>
    /// Converts a <see cref="UInt128"/> to a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UInt256(UInt128 value)
        => new UInt256(unchecked((ulong) value), (ulong) (value >> 64));

    /// <summary>
    /// Converts an <see cref="SByte"/> modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(sbyte value)
        => unchecked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="SByte"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(sbyte value)
        => checked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int16"/> modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(short value)
        => unchecked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int16"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(short value)
        => checked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int32"/> modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(int value)
        => unchecked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int32"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(int value)
        => checked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int64"/> modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(long value)
        => unchecked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int64"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(long value)
        => checked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int128"/> modulo 2^256.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(Int128 value)
        => unchecked((UInt256) (Int256) value);

    /// <summary>
    /// Converts an <see cref="Int128"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(Int128 value)
        => checked((UInt256) (Int256) value);

    /// <summary>
    /// Reinterprets the two's-complement bits of an <see cref="Int256"/> as a <see cref="UInt256"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(Int256 value)
        => value._value;

    /// <summary>
    /// Converts an <see cref="Int256"/> to a <see cref="UInt256"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(Int256 value)
        => value.IsNegative
            ? throw new OverflowException("Cannot convert a negative Int256 to UInt256.")
            : unchecked((UInt256) value);

    /// <summary>
    /// Interprets big-endian bytes as an unsigned 256-bit integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(in Bytes32 value)
        => BinaryPrimitives.ReadUInt256BigEndian(value.DangerousGetReadOnlySpan());

    /// <summary>
    /// Converts a <see cref="BigInteger"/> to a <see cref="UInt256"/>.
    /// Checks for overflow even in an unchecked context.
    /// </summary>
    /// <exception cref="OverflowException">The value is outside [0, 2^256 - 1].</exception>
    public static explicit operator UInt256(in BigInteger value)
    {
        Span<byte> bytes = stackalloc byte[32];
        bytes.Clear();
        return !value.TryWriteBytes(bytes, out _, isUnsigned: true, isBigEndian: false)
            ? throw new OverflowException("Cannot convert BigInteger value to UInt256.")
            : BinaryPrimitives.ReadUInt256LittleEndian(bytes);
    }

    /// <summary>
    /// Converts a <see cref="Decimal"/> to a <see cref="UInt256"/>, truncating toward zero.
    /// Checks for overflow even in an unchecked context.
    /// </summary>
    /// <exception cref="OverflowException">The truncated value is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(decimal value)
        => (UInt256) (UInt128) value;

    /// <summary>
    /// Converts a <see cref="Double"/> to a <see cref="UInt256"/>, truncating toward zero and saturating on overflow.
    /// Negative values and NaN become zero; values at or above 2^256, including positive infinity,
    /// become <see cref="MaxValue"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt256(double value)
    {
        if(!(value >= 1))
        {
            return default;
        }
        if(value < DOUBLE_UINT64_UPPER_BOUND)
        {
            return new UInt256(unchecked((ulong) value));
        }
        if(value >= DOUBLE_UPPER_BOUND)
        {
            return MaxValue;
        }

        ulong bits = BitConverter.DoubleToUInt64Bits(value);
        int exponent = (int) ((bits >> 52) & 0x7FF) - 1023;
        ulong significand = (bits & 0x000F_FFFF_FFFF_FFFFUL) | (1UL << 52);
        return FromUpstream(new CoreUInt256(significand) << (exponent - 52));
    }

    /// <summary>
    /// Converts a <see cref="Double"/> to a <see cref="UInt256"/>, truncating toward zero.
    /// </summary>
    /// <exception cref="OverflowException">
    /// The value is negative, NaN, infinite, or at least 2^256.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt256(double value)
        => value >= 0 && value < DOUBLE_UPPER_BOUND
            ? unchecked((UInt256) value)
            : throw new OverflowException("Cannot convert double value to UInt256.");

    /// <summary>
    /// Converts to an <see cref="SByte"/>, retaining the low 8 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator sbyte(in UInt256 value)
        => unchecked((sbyte) value._u0);

    /// <summary>
    /// Converts to an <see cref="SByte"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds SByte.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked sbyte(in UInt256 value)
        => checked((sbyte) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="Byte"/>, retaining the low 8 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator byte(in UInt256 value)
        => unchecked((byte) value._u0);

    /// <summary>
    /// Converts to a <see cref="Byte"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Byte.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked byte(in UInt256 value)
        => checked((byte) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int16"/>, retaining the low 16 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator short(in UInt256 value)
        => unchecked((short) value._u0);

    /// <summary>
    /// Converts to an <see cref="Int16"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Int16.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked short(in UInt256 value)
        => checked((short) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="UInt16"/>, retaining the low 16 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ushort(in UInt256 value)
        => unchecked((ushort) value._u0);

    /// <summary>
    /// Converts to a <see cref="UInt16"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds UInt16.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked ushort(in UInt256 value)
        => checked((ushort) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="Char"/>, retaining the low 16 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator char(in UInt256 value)
        => unchecked((char) value._u0);

    /// <summary>
    /// Converts to a <see cref="Char"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Char.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked char(in UInt256 value)
        => checked((char) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int32"/>, retaining the low 32 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator int(in UInt256 value)
        => unchecked((int) value._u0);

    /// <summary>
    /// Converts to an <see cref="Int32"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Int32.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked int(in UInt256 value)
        => checked((int) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="UInt32"/>, retaining the low 32 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(in UInt256 value)
        => unchecked((uint) value._u0);

    /// <summary>
    /// Converts to a <see cref="UInt32"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds UInt32.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked uint(in UInt256 value)
        => checked((uint) (ulong) value);

    /// <summary>
    /// Converts to an <see cref="Int64"/>, retaining the low 64 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator long(in UInt256 value)
        => unchecked((long) value._u0);

    /// <summary>
    /// Converts to an <see cref="Int64"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Int64.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked long(in UInt256 value)
        => checked((long) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="UInt64"/>, retaining the low 64 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ulong(in UInt256 value)
        => value._u0;

    /// <summary>
    /// Converts to a <see cref="UInt64"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds UInt64.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked ulong(in UInt256 value)
        => (value._u1 | value._u2 | value._u3) != 0
            ? throw new OverflowException("Cannot convert UInt256 value to UInt64.")
            : value._u0;

    /// <summary>
    /// Converts to an <see cref="Int128"/>, retaining the low 128 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Int128(in UInt256 value)
        => unchecked((Int128) (UInt128) value);

    /// <summary>
    /// Converts to an <see cref="Int128"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Int128.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked Int128(in UInt256 value)
        => checked((Int128) (UInt128) value);

    /// <summary>
    /// Converts to a <see cref="UInt128"/>, retaining the low 128 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt128(in UInt256 value)
        => ((UInt128) value._u1 << 64) | value._u0;

    /// <summary>
    /// Converts to a <see cref="UInt128"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds UInt128.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked UInt128(in UInt256 value)
        => (value._u2 | value._u3) != 0
            ? throw new OverflowException("Cannot convert UInt256 value to UInt128.")
            : unchecked((UInt128) value);

    /// <summary>
    /// Converts to an <see cref="IntPtr"/>, retaining the native-width low bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nint(in UInt256 value)
        => unchecked((nint) value._u0);

    /// <summary>
    /// Converts to an <see cref="IntPtr"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds IntPtr.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked nint(in UInt256 value)
        => checked((nint) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="UIntPtr"/>, retaining the native-width low bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nuint(in UInt256 value)
        => unchecked((nuint) value._u0);

    /// <summary>
    /// Converts to a <see cref="UIntPtr"/>.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds UIntPtr.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator checked nuint(in UInt256 value)
        => checked((nuint) (ulong) value);

    /// <summary>
    /// Converts to a <see cref="BigInteger"/>.
    /// </summary>
    public static explicit operator BigInteger(in UInt256 value)
    {
        if((value._u1 | value._u2 | value._u3) == 0)
        {
            return new BigInteger(value._u0);
        }

        var bytes = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<UInt256, byte>(ref Unsafe.AsRef(in value)), 32);
        return new BigInteger(bytes, isUnsigned: true, isBigEndian: false);
    }

    /// <summary>
    /// Converts to a <see cref="Double"/>, rounding to the nearest value with ties rounded to even.
    /// </summary>
    public static explicit operator double(in UInt256 value)
    {
        ulong upper;
        ulong lower;
        ulong discarded;
        int offset;
        if(value._u3 != 0)
        {
            upper = value._u3;
            lower = value._u2;
            discarded = value._u0 | value._u1;
            offset = 192;
        }
        else if(value._u2 != 0)
        {
            upper = value._u2;
            lower = value._u1;
            discarded = value._u0;
            offset = 128;
        }
        else if(value._u1 != 0)
        {
            upper = value._u1;
            lower = value._u0;
            discarded = 0;
            offset = 64;
        }
        else
        {
            return value._u0;
        }

        int left = BitOperations.LeadingZeroCount(upper);
        ulong leading;
        if(left == 0)
        {
            leading = upper;
            discarded |= lower;
        }
        else
        {
            leading = (upper << left) | (lower >> (64 - left));
            discarded |= lower << left;
        }

        leading |= discarded != 0 ? 1UL : 0;
        int shift = offset - left;
        return leading * BitConverter.UInt64BitsToDouble((ulong) (shift + 1023) << 52);
    }

    /// <summary>
    /// Converts to a <see cref="Single"/> through <see cref="Double"/>.
    /// Values that round beyond the finite range become positive infinity in either context.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator float(in UInt256 value)
        => (float) (double) value;

    /// <summary>
    /// Converts to a <see cref="Half"/> through <see cref="Double"/>.
    /// Values that round beyond the finite range become positive infinity in either context.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Half(in UInt256 value)
        => (Half) (double) value;

    /// <summary>
    /// Converts to a <see cref="Decimal"/>.
    /// Checks for overflow even in an unchecked context.
    /// </summary>
    /// <exception cref="OverflowException">The value exceeds Decimal.MaxValue.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator decimal(in UInt256 value)
        => checked((decimal) (UInt128) value);

    /// <summary>
    /// Represents the unsigned value as exactly 32 big-endian bytes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Bytes32(in UInt256 value)
    {
        Unsafe.SkipInit(out Bytes32 result);
        var bytes = MemoryMarshal.CreateSpan(ref Unsafe.As<Bytes32, byte>(ref result), Bytes32.BYTE_LENGTH);
        BinaryPrimitives.WriteUInt256BigEndian(bytes, in value);
        return result;
    }
}
