using EtherSharp.ABI.Packed;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Packed;
public class AbiNumberEncodingTests
{
    private readonly PackedAbiEncoder _encoder;

    public AbiNumberEncodingTests()
    {
        _encoder = new PackedAbiEncoder();
    }

    public static TheoryData<int> BitSizes
        => [.. Enumerable.Range(1, 32).Select(x => x * 8)];
    public static TheoryData<int> NonNativeBitSizes
        => [.. Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)];

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MinValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        expected[^(bitSize / 8)] = 128;

        byte[] actual = (bitSize switch
        {
            8 => _encoder.Number(SByte.MinValue, false, bitSize),
            16 => _encoder.Number(Int16.MinValue, false, bitSize),
            > 16 and <= 32 => _encoder.Number(Int32.MinValue >> (32 - bitSize), false, bitSize),
            > 32 and <= 64 => _encoder.Number(Int64.MinValue >> (64 - bitSize), false, bitSize),
            > 64 and <= 256 => _encoder.Number(-BigInteger.Pow(2, bitSize - 1), false, bitSize),
            _ => throw new NotSupportedException()
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MaxValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        _ = ((BigInteger.One << (bitSize - 1)) - 1)
            .TryWriteBytes(expected.AsSpan(), out _, false, true);

        byte[] actual = (bitSize switch
        {
            8 => _encoder.Number(SByte.MaxValue, false, bitSize),
            16 => _encoder.Number(Int16.MaxValue, false, bitSize),
            > 16 and <= 32 => _encoder.Number(Int32.MaxValue >> (32 - bitSize), false, bitSize),
            > 32 and <= 64 => _encoder.Number(Int64.MaxValue >> (64 - bitSize), false, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Pow(2, bitSize - 1) - 1, false, bitSize),
            _ => throw new NotSupportedException()
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MaxValue_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        expected.AsSpan().Fill(255);

        byte[] actual = (bitSize switch
        {
            8 => _encoder.Number(Byte.MaxValue, true, bitSize),
            16 => _encoder.Number(UInt16.MaxValue, true, bitSize),
            > 16 and <= 32 => _encoder.Number(UInt32.MaxValue >> (32 - bitSize), true, bitSize),
            > 32 and <= 64 => _encoder.Number(UInt64.MaxValue >> (64 - bitSize), true, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Pow(2, bitSize) - 1, true, bitSize),
            _ => throw new NotSupportedException()
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_Zero_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        byte[] actual = (bitSize switch
        {
            8 => _encoder.Number((sbyte) 0, false, bitSize),
            16 => _encoder.Number((short) 0, false, bitSize),
            > 16 and <= 32 => _encoder.Number(0, false, bitSize),
            > 32 and <= 64 => _encoder.Number((long) 0, false, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Zero, false, bitSize),
            _ => throw new NotSupportedException()
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_Zero_Output(int bitSize)
    {
        byte[] expected = new byte[bitSize / 8];
        byte[] actual = (bitSize switch
        {
            8 => _encoder.Number((byte) 0, true, bitSize),
            16 => _encoder.Number((ushort) 0, true, bitSize),
            > 16 and <= 32 => _encoder.Number((uint) 0, true, bitSize),
            > 32 and <= 64 => _encoder.Number((ulong) 0, true, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Zero, true, bitSize),
            _ => throw new NotSupportedException()
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(NonNativeBitSizes))]
    public void Should_Throw_On_Int_Below_Minimum(int bitSize)
        => Assert.Throws<ArgumentException>(() => bitSize switch
        {
            24 => _encoder.Number((Int32.MinValue >> (32 - bitSize)) - 1, false, bitSize),
            > 32 and < 64 => _encoder.Number((Int64.MinValue >> (64 - bitSize)) - 1, false, bitSize),
            > 64 and <= 256 => _encoder.Number(-BigInteger.Pow(2, bitSize - 1) - 1, false, bitSize),
            _ => throw new NotSupportedException()
        });

    [Theory]
    [MemberData(nameof(NonNativeBitSizes))]
    public void Should_Throw_On_UInt_AboveMaximum(int bitSize)
        => Assert.Throws<ArgumentException>(() => bitSize switch
        {
            24 => _encoder.Number((UInt32.MaxValue >> (32 - bitSize)) + 1, true, bitSize),
            > 32 and < 64 => _encoder.Number((UInt64.MaxValue >> (64 - bitSize)) + 1, true, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Pow(2, bitSize), true, bitSize),
            _ => throw new NotSupportedException()
        });

    [Theory]
    [MemberData(nameof(NonNativeBitSizes))]
    public void Should_Throw_On_Int_Above_Maximum(int bitSize)
        => Assert.Throws<ArgumentException>(() => bitSize switch
        {
            24 => _encoder.Number((Int32.MaxValue >> (32 - bitSize)) + 1, false, bitSize),
            > 32 and < 64 => _encoder.Number((Int64.MaxValue >> (64 - bitSize)) + 1, false, bitSize),
            > 64 and <= 256 => _encoder.Number(BigInteger.Pow(2, bitSize - 1), false, bitSize),
            _ => throw new NotSupportedException()
        });
}
