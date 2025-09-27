using EtherSharp.ABI.Packed;

namespace EtherSharp.Tests.ABI.Packed;
public class StringAbiEncoderTests
{
    private readonly PackedAbiEncoder _encoder;

    public StringAbiEncoderTests()
    {
        _encoder = new PackedAbiEncoder();
    }

    [Fact]
    public void Should_Match_HelloWorld_Output()
    {
        string input = "hello_world";
        byte[] expected = Convert.FromHexString("68656c6c6f5f776f726c64");
        byte[] actual = _encoder.String(input).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Match_LongAlphabet_Output()
    {
        string input = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] expected = Convert.FromHexString("6162636465666768696a6b6c6d6e6f707172737475767778797a4142434445464748494a4b4c4d4e4f505152535455565758595a6162636465666768696a6b6c6d6e6f707172737475767778797a4142434445464748494a4b4c4d4e4f505152535455565758595a6162636465666768696a6b6c6d6e6f707172737475767778797a4142434445464748494a4b4c4d4e4f505152535455565758595a");
        byte[] actual = _encoder.String(input).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Match_EmptyString_Output()
    {
        string input = String.Empty;
        byte[] expected = Convert.FromHexString("");
        byte[] actual = _encoder.String(input).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Throw_On_Null()
        => Assert.Throws<ArgumentNullException>(() => _encoder.String(null!));
}