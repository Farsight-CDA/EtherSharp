using EtherSharp.Types;

namespace EtherSharp.Tests.Types;

public class AddressTests
{
    [Fact]
    public void Should_Compare_Using_Solidity_Address_Order()
    {
        var low = Address.Parse("0x0000000000000000000000000000000000000001");
        var sameAsLow = Address.Parse("0x0000000000000000000000000000000000000001");
        var high = Address.Parse("0x0000000000000000000000000000000000000002");
        var muchHigher = Address.Parse("0x0100000000000000000000000000000000000000");

        Assert.True(low < high);
        Assert.True(low <= high);
        Assert.True(high > low);
        Assert.True(high >= low);
        Assert.True(muchHigher > high);
        Assert.True(low <= sameAsLow);
        Assert.True(low >= sameAsLow);

        Assert.False(high < low);
        Assert.False(low > high);
    }
}
