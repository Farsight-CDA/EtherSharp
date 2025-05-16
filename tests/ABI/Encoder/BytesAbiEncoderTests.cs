using EtherSharp.ABI;
using System.Security.Cryptography;

namespace EtherSharp.Tests.ABI.Encoder;

public class BytesAbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public BytesAbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    public static TheoryData<int> BitSizes
        => [.. Enumerable.Range(1, 32).Select(x => x * 8)];
    public static TheoryData<int> NonNativeBitSizes
        => [.. Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)];

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Random1(int bitSize)
    {
        var rng = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[bitSize];

        rng.GetBytes(randomBytes);

        byte[] actual = _encoder.Bytes(randomBytes).Build();

        Assert.Equal(randomBytes, actual[64..(64 + bitSize)]);

    }

    [Theory]
    [MemberData(nameof(NonNativeBitSizes))]
    public void Should_Match_Random2(int nonNativeBitSizes)
    {
        var rng = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[nonNativeBitSizes];

        rng.GetBytes(randomBytes);

        byte[] actual = _encoder.Bytes(randomBytes).Build();

        Assert.Equal(randomBytes, actual[64..(64 + nonNativeBitSizes)]);

    }
}
