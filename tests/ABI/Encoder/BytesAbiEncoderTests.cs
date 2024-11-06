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
    public void Should_Match_Random1(uint bitSize)
    {
        var rng = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[bitSize];

        rng.GetBytes(randomBytes);

        byte[] actual = _encoder.Bytes(randomBytes).Build();

        Assert.Equal(randomBytes, actual[64..(64 + (int) bitSize)]);

    }

    [Theory]
    [MemberData(nameof(NonNativeBitSizes))]
    public void Should_Match_Random2(uint nonNativeBitSizes)
    {
        var rng = RandomNumberGenerator.Create();

        byte[] randomBytes = new byte[nonNativeBitSizes];

        rng.GetBytes(randomBytes);

        byte[] actual = _encoder.Bytes(randomBytes).Build();

        Assert.Equal(randomBytes, actual[64..(64 + (int) nonNativeBitSizes)]);

    }
}
