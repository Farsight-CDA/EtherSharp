using EtherSharp.ABI;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Encoder;
public class NumberArrayAbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public NumberArrayAbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    public static IEnumerable<object[]> BitSizes
        => Enumerable.Range(1, 32)
            .Select(x => new object[] { x * 8 });
    public static IEnumerable<object[]> NonNativeBitSizes
        => Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)
            .Select(x => new object[] { x }
        );

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Empty_Int_Array_Output(int bitSize)
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = (bitSize switch
        {
            8 => _encoder.NumberArray<sbyte>(false, bitSize),
            16 => _encoder.NumberArray<short>(false, bitSize),
            > 16 and <= 32 => _encoder.NumberArray<int>(false, bitSize),
            > 32 and <= 64 => _encoder.NumberArray<long>(false, bitSize),
            > 64 and <= 256 => _encoder.NumberArray<BigInteger>(false, bitSize),
            _ => throw new NotSupportedException()
        }).Build();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Empty_UInt_Array_Output(int bitSize)
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = (bitSize switch
        {
            8 => _encoder.NumberArray<byte>(true, bitSize),
            16 => _encoder.NumberArray<ushort>(true, bitSize),
            > 16 and <= 32 => _encoder.NumberArray<uint>(true, bitSize),
            > 32 and <= 64 => _encoder.NumberArray<ulong>(true, bitSize),
            > 64 and <= 256 => _encoder.NumberArray<BigInteger>(true, bitSize),
            _ => throw new NotSupportedException()
        }).Build();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_ZeroElement_UInt_Array_Output(int bitSize)
    {
        byte[] expected = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = (bitSize switch
        {
            8 => _encoder.NumberArray<byte>(true, bitSize, 0, 0, 0, 0),
            16 => _encoder.NumberArray<ushort>(true, bitSize, 0, 0, 0, 0),
            > 16 and <= 32 => _encoder.NumberArray<uint>(true, bitSize, 0, 0, 0, 0),
            > 32 and <= 64 => _encoder.NumberArray<ulong>(true, bitSize, 0, 0, 0, 0),
            > 64 and <= 256 => _encoder.NumberArray<BigInteger>(true, bitSize, 0, 0, 0, 0),
            _ => throw new NotSupportedException()
        }).Build();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_ZeroElement_Int_Array_Output(int bitSize)
    {
        byte[] expected = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = (bitSize switch
        {
            8 => _encoder.NumberArray<sbyte>(false, bitSize, 0, 0, 0, 0),
            16 => _encoder.NumberArray<short>(false, bitSize, 0, 0, 0, 0),
            > 16 and <= 32 => _encoder.NumberArray(false, bitSize, 0, 0, 0, 0),
            > 32 and <= 64 => _encoder.NumberArray<long>(false, bitSize, 0, 0, 0, 0),
            > 64 and <= 256 => _encoder.NumberArray<BigInteger>(false, bitSize, 0, 0, 0, 0),
            _ => throw new NotSupportedException()
        }).Build();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_VaryingElements_Int_Array_Output(int bitSize)
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000005ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffcefffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff60000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000032");
        byte[] actual = (bitSize switch
        {
            8 => _encoder.NumberArray<sbyte>(false, bitSize, -50, -10, 0, 10, 50),
            16 => _encoder.NumberArray<short>(false, bitSize, -50, -10, 0, 10, 50),
            > 16 and <= 32 => _encoder.NumberArray(false, bitSize, -50, -10, 0, 10, 50),
            > 32 and <= 64 => _encoder.NumberArray<long>(false, bitSize, -50, -10, 0, 10, 50),
            > 64 and <= 256 => _encoder.NumberArray<BigInteger>(false, bitSize, -50, -10, 0, 10, 50),
            _ => throw new NotSupportedException()
        }).Build();
        Assert.Equal(expected, actual);
    }
}
