using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.Decoder;

public class BytesAbiDecoderTests
{
    public static TheoryData<int> BitSizes
        => CreateBitSizesData(Enumerable.Range(1, 32).Select(x => x * 8));

    public static TheoryData<int> NonNativeBitSizes
        => CreateBitSizesData(
            Enumerable.Range(1, 32)
                .Select(x => x * 8)
                .Where(x => x != 8 && x != 16 && x != 32 && x != 64)
        );

    private static TheoryData<int> CreateBitSizesData(IEnumerable<int> bitSizes)
    {
        var data = new TheoryData<int>();

        foreach(int bitSize in bitSizes)
        {
            data.Add(bitSize);
        }

        return data;
    }

    [Fact]
    public void Should_Match()
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000000");

        var actualBytes = new AbiDecoder(input).Bytes();

        Assert.Equal([0, 0, 0], actualBytes.ToArray());
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Full_Zeros_Output(int bitSize)
    {
        byte[] input = new byte[32];

        var output = new AbiDecoder(input).SizedBytes(bitSize);

        Assert.Equal(input.AsSpan()[0..(bitSize / 8)], output.Span);
    }
}
