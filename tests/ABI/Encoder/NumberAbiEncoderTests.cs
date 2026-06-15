using EtherSharp.ABI;
using EtherSharp.Numerics;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Encoder;

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
        byte[] expected = new byte[32];
        expected.AsSpan()[0..(32 - (bitSize / 8))].Fill(255);
        expected[^(bitSize / 8)] = 128;

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
        byte[] expected = new byte[32];
        _ = ((BigInteger.One << (bitSize - 1)) - 1)
            .TryWriteBytes(expected.AsSpan()[(32 - (bitSize / 8))..], out _, false, true);

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
        byte[] expected = new byte[32];
        expected.AsSpan(32 - (bitSize / 8)).Fill(255);

        object value = bitSize switch
        {
            8 => Byte.MaxValue,
            16 => UInt16.MaxValue,
            > 16 and <= 32 => UInt32.MaxValue >> (32 - bitSize),
            > 32 and <= 64 => UInt64.MaxValue >> (64 - bitSize),
            > 64 and < 256 => UInt256.Pow(2, (uint) bitSize) - 1,
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
        byte[] expected = new byte[32];
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
        byte[] expected = new byte[32];
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
            8 => new AbiEncoder().Int8((sbyte) value),
            16 => new AbiEncoder().Int16((short) value),
            24 => new AbiEncoder().Int24((int) value),
            32 => new AbiEncoder().Int32((int) value),
            40 => new AbiEncoder().Int40((long) value),
            48 => new AbiEncoder().Int48((long) value),
            56 => new AbiEncoder().Int56((long) value),
            64 => new AbiEncoder().Int64((long) value),
            72 => new AbiEncoder().Int72((Int256) value),
            80 => new AbiEncoder().Int80((Int256) value),
            88 => new AbiEncoder().Int88((Int256) value),
            96 => new AbiEncoder().Int96((Int256) value),
            104 => new AbiEncoder().Int104((Int256) value),
            112 => new AbiEncoder().Int112((Int256) value),
            120 => new AbiEncoder().Int120((Int256) value),
            128 => new AbiEncoder().Int128((Int256) value),
            136 => new AbiEncoder().Int136((Int256) value),
            144 => new AbiEncoder().Int144((Int256) value),
            152 => new AbiEncoder().Int152((Int256) value),
            160 => new AbiEncoder().Int160((Int256) value),
            168 => new AbiEncoder().Int168((Int256) value),
            176 => new AbiEncoder().Int176((Int256) value),
            184 => new AbiEncoder().Int184((Int256) value),
            192 => new AbiEncoder().Int192((Int256) value),
            200 => new AbiEncoder().Int200((Int256) value),
            208 => new AbiEncoder().Int208((Int256) value),
            216 => new AbiEncoder().Int216((Int256) value),
            224 => new AbiEncoder().Int224((Int256) value),
            232 => new AbiEncoder().Int232((Int256) value),
            240 => new AbiEncoder().Int240((Int256) value),
            248 => new AbiEncoder().Int248((Int256) value),
            256 => new AbiEncoder().Int256((Int256) value),
            _ => throw new NotSupportedException()
        }).Build();

    private static byte[] EncodeUInt(int bitSize, object value)
        => (bitSize switch
        {
            8 => new AbiEncoder().UInt8((byte) value),
            16 => new AbiEncoder().UInt16((ushort) value),
            24 => new AbiEncoder().UInt24((uint) value),
            32 => new AbiEncoder().UInt32((uint) value),
            40 => new AbiEncoder().UInt40((ulong) value),
            48 => new AbiEncoder().UInt48((ulong) value),
            56 => new AbiEncoder().UInt56((ulong) value),
            64 => new AbiEncoder().UInt64((ulong) value),
            72 => new AbiEncoder().UInt72((UInt256) value),
            80 => new AbiEncoder().UInt80((UInt256) value),
            88 => new AbiEncoder().UInt88((UInt256) value),
            96 => new AbiEncoder().UInt96((UInt256) value),
            104 => new AbiEncoder().UInt104((UInt256) value),
            112 => new AbiEncoder().UInt112((UInt256) value),
            120 => new AbiEncoder().UInt120((UInt256) value),
            128 => new AbiEncoder().UInt128((UInt256) value),
            136 => new AbiEncoder().UInt136((UInt256) value),
            144 => new AbiEncoder().UInt144((UInt256) value),
            152 => new AbiEncoder().UInt152((UInt256) value),
            160 => new AbiEncoder().UInt160((UInt256) value),
            168 => new AbiEncoder().UInt168((UInt256) value),
            176 => new AbiEncoder().UInt176((UInt256) value),
            184 => new AbiEncoder().UInt184((UInt256) value),
            192 => new AbiEncoder().UInt192((UInt256) value),
            200 => new AbiEncoder().UInt200((UInt256) value),
            208 => new AbiEncoder().UInt208((UInt256) value),
            216 => new AbiEncoder().UInt216((UInt256) value),
            224 => new AbiEncoder().UInt224((UInt256) value),
            232 => new AbiEncoder().UInt232((UInt256) value),
            240 => new AbiEncoder().UInt240((UInt256) value),
            248 => new AbiEncoder().UInt248((UInt256) value),
            256 => new AbiEncoder().UInt256((UInt256) value),
            _ => throw new NotSupportedException()
        }).Build();
}
