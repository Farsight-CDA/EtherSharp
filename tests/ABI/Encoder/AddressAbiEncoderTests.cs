using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.Encoder;
public class AddressAbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public AddressAbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Should_Match_Null_Address_Output()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Address("0x0000000000000000000000000000000000000000").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Match_UpperCase_Address_Output()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000004838b106fce9647bdf1e7877bf73ce8b0bad5f97");
        byte[] actual = _encoder.Address("0x4838B106FCE9647BDF1E7877BF73CE8B0BAD5F97").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Match_LowerCase_Address_Output()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000004838b106fce9647bdf1e7877bf73ce8b0bad5f97");
        byte[] actual = _encoder.Address("0x4838b106fce9647bdf1e7877bf73ce8b0bad5f97").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Match_MixedCase_Address_Output()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000004838b106fce9647bdf1e7877bf73ce8b0bad5f97");
        byte[] actual = _encoder.Address("0x4838b106fCe9647BdF1E7877bF73cE8B0BaD5F97").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Throw_On_Missing_0x_Prefix()
        => Assert.Throws<ArgumentException>(() => _encoder.Address("0000000000000000000000000000000000000000"));
}
