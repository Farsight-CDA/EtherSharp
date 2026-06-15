using EtherSharp.ABI;
using EtherSharp.Numerics;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Decoder;

public sealed class AbiSizedNumberDecoderTests
{
    public static TheoryData<int> BitSizes
        => [.. Enumerable.Range(1, 32).Select(static x => x * 8)];

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MinValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        input.AsSpan()[0..(32 - (bitSize / 8))].Fill(255);
        input[^(bitSize / 8)] = 128;

        object expected = bitSize switch
        {
            8 => SByte.MinValue,
            16 => Int16.MinValue,
            > 16 and <= 32 => Int32.MinValue >> (32 - bitSize),
            > 32 and <= 64 => Int64.MinValue >> (64 - bitSize),
            > 64 and <= 256 => -Int256.Pow(2, bitSize - 1),
            _ => throw new NotSupportedException()
        };

        Assert.Equal(expected, DecodeInt(input, bitSize));
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MaxValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        _ = ((BigInteger.One << (bitSize - 1)) - 1)
            .TryWriteBytes(input.AsSpan()[(32 - (bitSize / 8))..], out _, false, true);

        object expected = bitSize switch
        {
            8 => SByte.MaxValue,
            16 => Int16.MaxValue,
            > 16 and <= 32 => Int32.MaxValue >> (32 - bitSize),
            > 32 and <= 64 => Int64.MaxValue >> (64 - bitSize),
            > 64 and <= 256 => Int256.Pow(2, bitSize - 1) - 1,
            _ => throw new NotSupportedException()
        };

        Assert.Equal(expected, DecodeInt(input, bitSize));
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MinValue_Output(int bitSize)
    {
        byte[] input = new byte[32];

        object expected = bitSize switch
        {
            8 => Byte.MinValue,
            16 => UInt16.MinValue,
            > 16 and <= 32 => UInt32.MinValue,
            > 32 and <= 64 => UInt64.MinValue,
            > 64 and <= 256 => (object) UInt256.Zero,
            _ => throw new NotSupportedException()
        };

        Assert.Equal(expected, DecodeUInt(input, bitSize));
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MaxValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        input.AsSpan(32 - (bitSize / 8)).Fill(255);

        object expected = bitSize switch
        {
            8 => Byte.MaxValue,
            16 => UInt16.MaxValue,
            > 16 and <= 32 => UInt32.MaxValue >> (32 - bitSize),
            > 32 and <= 64 => UInt64.MaxValue >> (64 - bitSize),
            > 64 and < 256 => (object) (UInt256.Pow(2, (uint) bitSize) - 1),
            256 => UInt256.MaxValue,
            _ => throw new NotSupportedException()
        };

        Assert.Equal(expected, DecodeUInt(input, bitSize));
    }

    private static object DecodeInt(byte[] input, int bitSize)
    {
        var decoder = new AbiDecoder(input);
        return bitSize switch
        {
            8 => decoder.Int8(),
            16 => decoder.Int16(),
            24 => decoder.Int24(),
            32 => decoder.Int32(),
            40 => decoder.Int40(),
            48 => decoder.Int48(),
            56 => decoder.Int56(),
            64 => decoder.Int64(),
            72 => decoder.Int72(),
            80 => decoder.Int80(),
            88 => decoder.Int88(),
            96 => decoder.Int96(),
            104 => decoder.Int104(),
            112 => decoder.Int112(),
            120 => decoder.Int120(),
            128 => decoder.Int128(),
            136 => decoder.Int136(),
            144 => decoder.Int144(),
            152 => decoder.Int152(),
            160 => decoder.Int160(),
            168 => decoder.Int168(),
            176 => decoder.Int176(),
            184 => decoder.Int184(),
            192 => decoder.Int192(),
            200 => decoder.Int200(),
            208 => decoder.Int208(),
            216 => decoder.Int216(),
            224 => decoder.Int224(),
            232 => decoder.Int232(),
            240 => decoder.Int240(),
            248 => decoder.Int248(),
            256 => decoder.Int256(),
            _ => throw new NotSupportedException()
        };
    }

    private static object DecodeUInt(byte[] input, int bitSize)
    {
        var decoder = new AbiDecoder(input);
        return bitSize switch
        {
            8 => decoder.UInt8(),
            16 => decoder.UInt16(),
            24 => decoder.UInt24(),
            32 => decoder.UInt32(),
            40 => decoder.UInt40(),
            48 => decoder.UInt48(),
            56 => decoder.UInt56(),
            64 => decoder.UInt64(),
            72 => (object) decoder.UInt72(),
            80 => decoder.UInt80(),
            88 => decoder.UInt88(),
            96 => decoder.UInt96(),
            104 => decoder.UInt104(),
            112 => decoder.UInt112(),
            120 => decoder.UInt120(),
            128 => decoder.UInt128(),
            136 => decoder.UInt136(),
            144 => decoder.UInt144(),
            152 => decoder.UInt152(),
            160 => decoder.UInt160(),
            168 => decoder.UInt168(),
            176 => decoder.UInt176(),
            184 => decoder.UInt184(),
            192 => decoder.UInt192(),
            200 => decoder.UInt200(),
            208 => decoder.UInt208(),
            216 => decoder.UInt216(),
            224 => decoder.UInt224(),
            232 => decoder.UInt232(),
            240 => decoder.UInt240(),
            248 => decoder.UInt248(),
            256 => decoder.UInt256(),
            _ => throw new NotSupportedException()
        };
    }
}
