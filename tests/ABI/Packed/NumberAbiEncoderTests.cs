using EtherSharp.ABI.Packed;
using EtherSharp.Numerics;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Packed;

public sealed class AbiSizedNumberEncodingTests
{
    public static TheoryData<int> BitSizes
        => [.. Enumerable.Range(1, 32).Select(static x => x * 8)];

    public static TheoryData<int> NonNativeBoundedBitSizes
        => [.. Enumerable.Range(1, 32)
            .Select(static x => x * 8)
            .Where(static x => x is not (8 or 16 or 32 or 64 or 256))];

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MinValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        expected[0] = 128;

        object value = bitSize switch
        {
            8 => SByte.MinValue,
            16 => Int16.MinValue,
            > 16 and <= 32 => Int32.MinValue >> (32 - bitSize),
            > 32 and <= 64 => Int64.MinValue >> (64 - bitSize),
            > 64 and <= 256 => -Int256.Pow(2, bitSize - 1),
            _ => throw new NotSupportedException()
        };

        byte[] actual = EncodeInt(bitSize, value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MaxValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        _ = ((BigInteger.One << (bitSize - 1)) - 1)
            .TryWriteBytes(expected.AsSpan(), out _, false, true);

        object value = bitSize switch
        {
            8 => SByte.MaxValue,
            16 => Int16.MaxValue,
            > 16 and <= 32 => Int32.MaxValue >> (32 - bitSize),
            > 32 and <= 64 => Int64.MaxValue >> (64 - bitSize),
            > 64 and <= 256 => Int256.Pow(2, bitSize - 1) - 1,
            _ => throw new NotSupportedException()
        };

        byte[] actual = EncodeInt(bitSize, value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MaxValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        expected.AsSpan().Fill(255);

        object value = bitSize switch
        {
            8 => Byte.MaxValue,
            16 => UInt16.MaxValue,
            > 16 and <= 32 => UInt32.MaxValue >> (32 - bitSize),
            > 32 and <= 64 => UInt64.MaxValue >> (64 - bitSize),
            > 64 and < 256 => (object) (UInt256.Pow(2, (uint) bitSize) - 1),
            256 => UInt256.MaxValue,
            _ => throw new NotSupportedException()
        };

        byte[] actual = EncodeUInt(bitSize, value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_Zero_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        object value = bitSize switch
        {
            8 => (sbyte) 0,
            16 => (short) 0,
            > 16 and <= 32 => 0,
            > 32 and <= 64 => (long) 0,
            > 64 and <= 256 => Int256.Zero,
            _ => throw new NotSupportedException()
        };

        byte[] actual = EncodeInt(bitSize, value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_Zero_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        object value = bitSize switch
        {
            8 => (byte) 0,
            16 => (ushort) 0,
            > 16 and <= 32 => (uint) 0,
            > 32 and <= 64 => (ulong) 0,
            > 64 and <= 256 => (object) UInt256.Zero,
            _ => throw new NotSupportedException()
        };

        byte[] actual = EncodeUInt(bitSize, value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(NonNativeBoundedBitSizes))]
    public void Should_Throw_On_Int_Below_Minimum(int bitSize)
    {
        object value = bitSize switch
        {
            24 => (Int32.MinValue >> (32 - bitSize)) - 1,
            > 32 and < 64 => (Int64.MinValue >> (64 - bitSize)) - 1,
            > 64 and < 256 => -Int256.Pow(2, bitSize - 1) - 1,
            _ => throw new NotSupportedException()
        };

        Assert.Throws<ArgumentException>(() => EncodeInt(bitSize, value));
    }

    [Theory]
    [MemberData(nameof(NonNativeBoundedBitSizes))]
    public void Should_Throw_On_UInt_AboveMaximum(int bitSize)
    {
        object value = bitSize switch
        {
            24 => (UInt32.MaxValue >> (32 - bitSize)) + 1u,
            > 32 and < 64 => (UInt64.MaxValue >> (64 - bitSize)) + 1ul,
            > 64 and < 256 => (object) UInt256.Pow(2, (uint) bitSize),
            _ => throw new NotSupportedException()
        };

        Assert.Throws<ArgumentException>(() => EncodeUInt(bitSize, value));
    }

    [Theory]
    [MemberData(nameof(NonNativeBoundedBitSizes))]
    public void Should_Throw_On_Int_Above_Maximum(int bitSize)
    {
        object value = bitSize switch
        {
            24 => (Int32.MaxValue >> (32 - bitSize)) + 1,
            > 32 and < 64 => (Int64.MaxValue >> (64 - bitSize)) + 1,
            > 64 and < 256 => Int256.Pow(2, bitSize - 1),
            _ => throw new NotSupportedException()
        };

        Assert.Throws<ArgumentException>(() => EncodeInt(bitSize, value));
    }

    private static byte[] EncodeInt(int bitSize, object value)
        => (bitSize switch
        {
            8 => new PackedAbiEncoder().Int8((sbyte) value),
            16 => new PackedAbiEncoder().Int16((short) value),
            24 => new PackedAbiEncoder().Int24((int) value),
            32 => new PackedAbiEncoder().Int32((int) value),
            40 => new PackedAbiEncoder().Int40((long) value),
            48 => new PackedAbiEncoder().Int48((long) value),
            56 => new PackedAbiEncoder().Int56((long) value),
            64 => new PackedAbiEncoder().Int64((long) value),
            72 => new PackedAbiEncoder().Int72((Int256) value),
            80 => new PackedAbiEncoder().Int80((Int256) value),
            88 => new PackedAbiEncoder().Int88((Int256) value),
            96 => new PackedAbiEncoder().Int96((Int256) value),
            104 => new PackedAbiEncoder().Int104((Int256) value),
            112 => new PackedAbiEncoder().Int112((Int256) value),
            120 => new PackedAbiEncoder().Int120((Int256) value),
            128 => new PackedAbiEncoder().Int128((Int256) value),
            136 => new PackedAbiEncoder().Int136((Int256) value),
            144 => new PackedAbiEncoder().Int144((Int256) value),
            152 => new PackedAbiEncoder().Int152((Int256) value),
            160 => new PackedAbiEncoder().Int160((Int256) value),
            168 => new PackedAbiEncoder().Int168((Int256) value),
            176 => new PackedAbiEncoder().Int176((Int256) value),
            184 => new PackedAbiEncoder().Int184((Int256) value),
            192 => new PackedAbiEncoder().Int192((Int256) value),
            200 => new PackedAbiEncoder().Int200((Int256) value),
            208 => new PackedAbiEncoder().Int208((Int256) value),
            216 => new PackedAbiEncoder().Int216((Int256) value),
            224 => new PackedAbiEncoder().Int224((Int256) value),
            232 => new PackedAbiEncoder().Int232((Int256) value),
            240 => new PackedAbiEncoder().Int240((Int256) value),
            248 => new PackedAbiEncoder().Int248((Int256) value),
            256 => new PackedAbiEncoder().Int256((Int256) value),
            _ => throw new NotSupportedException()
        }).Build();

    private static byte[] EncodeUInt(int bitSize, object value)
        => (bitSize switch
        {
            8 => new PackedAbiEncoder().UInt8((byte) value),
            16 => new PackedAbiEncoder().UInt16((ushort) value),
            24 => new PackedAbiEncoder().UInt24((uint) value),
            32 => new PackedAbiEncoder().UInt32((uint) value),
            40 => new PackedAbiEncoder().UInt40((ulong) value),
            48 => new PackedAbiEncoder().UInt48((ulong) value),
            56 => new PackedAbiEncoder().UInt56((ulong) value),
            64 => new PackedAbiEncoder().UInt64((ulong) value),
            72 => new PackedAbiEncoder().UInt72((UInt256) value),
            80 => new PackedAbiEncoder().UInt80((UInt256) value),
            88 => new PackedAbiEncoder().UInt88((UInt256) value),
            96 => new PackedAbiEncoder().UInt96((UInt256) value),
            104 => new PackedAbiEncoder().UInt104((UInt256) value),
            112 => new PackedAbiEncoder().UInt112((UInt256) value),
            120 => new PackedAbiEncoder().UInt120((UInt256) value),
            128 => new PackedAbiEncoder().UInt128((UInt256) value),
            136 => new PackedAbiEncoder().UInt136((UInt256) value),
            144 => new PackedAbiEncoder().UInt144((UInt256) value),
            152 => new PackedAbiEncoder().UInt152((UInt256) value),
            160 => new PackedAbiEncoder().UInt160((UInt256) value),
            168 => new PackedAbiEncoder().UInt168((UInt256) value),
            176 => new PackedAbiEncoder().UInt176((UInt256) value),
            184 => new PackedAbiEncoder().UInt184((UInt256) value),
            192 => new PackedAbiEncoder().UInt192((UInt256) value),
            200 => new PackedAbiEncoder().UInt200((UInt256) value),
            208 => new PackedAbiEncoder().UInt208((UInt256) value),
            216 => new PackedAbiEncoder().UInt216((UInt256) value),
            224 => new PackedAbiEncoder().UInt224((UInt256) value),
            232 => new PackedAbiEncoder().UInt232((UInt256) value),
            240 => new PackedAbiEncoder().UInt240((UInt256) value),
            248 => new PackedAbiEncoder().UInt248((UInt256) value),
            256 => new PackedAbiEncoder().UInt256((UInt256) value),
            _ => throw new NotSupportedException()
        }).Build();
}
