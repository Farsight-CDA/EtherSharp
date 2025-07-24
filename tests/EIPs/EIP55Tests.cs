using EtherSharp.EIPs;
using EtherSharp.Types;

namespace EtherSharp.Tests.EIPs;
public class EIP55Tests
{
    [Fact]
    public void Should_Match_Zero_Address()
    {
        var address = Address.Zero;

        string expected = address.String;
        string actual = EIP55.FormatAddress(address);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Not_Change_Address_Content()
    {
        var rnd = new Random(1);
        Span<byte> buffer = stackalloc byte[20];

        for(int i = 0; i < 1000; i++)
        {
            rnd.NextBytes(buffer);
            var address = Address.FromBytes(buffer);

            string expected = address.String;
            string actual = EIP55.FormatAddress(address);

            Assert.Equal(expected, actual, true);
        }
    }

    [Theory]
    [InlineData("0x7E5F4552091A69125d5DfCb7b8C2659029395Bdf")]
    [InlineData("0x4a3FC77876ACFa7455D80aD1205464803D10A618")]
    [InlineData("0x124d778da06E0b94651CCfBddFF42BC6Ed37BD99")]
    [InlineData("0xD01A442D748ecbcba9b0C57c5c739A39c7e23172")]
    public void Should_Generate_Valid_EIP55(string formattedAddress)
    {
        var address = Address.FromString("0x" + formattedAddress[2..].ToUpper());
        string actual = EIP55.FormatAddress(address);

        Assert.Equal(formattedAddress, actual);
    }
}
