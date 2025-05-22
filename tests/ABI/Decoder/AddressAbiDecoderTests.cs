using EtherSharp.ABI;
using EtherSharp.Types;

namespace EtherSharp.Tests.ABI.Decoder;
public class AddressAbiDecoderTests
{
    [Fact]
    public void Should_Match_Null_Address_Output()
    {
        string input = "0000000000000000000000000000000000000000000000000000000000000000";
        var address = new AbiDecoder(Convert.FromHexString(input)).Address();
        Assert.Equal(Address.Zero, address);
    }

    [Fact]
    public void Should_Match_User_Address_Output()
    {
        string input = "0000000000000000000000004838b106fce9647bdf1e7877bf73ce8b0bad5f97";
        var address = new AbiDecoder(Convert.FromHexString(input)).Address();
        Assert.Equal(Address.FromString("0x4838B106FCE9647BDF1E7877BF73CE8B0BAD5F97"), address);
    }
}
