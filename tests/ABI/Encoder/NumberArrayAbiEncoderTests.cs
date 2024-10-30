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
}
