using EtherSharp.Types;

namespace EtherSharp.Tests.Types;

public class AddressTests
{
    [Fact]
    public void Should_Copy_Address_Bytes_Without_Exposing_Span()
    {
        byte[] expected = Convert.FromHexString("00112233445566778899AABBCCDDEEFF00112233");
        var address = Address.FromBytes(expected);

        Span<byte> copied = stackalloc byte[Address.BYTES_LENGTH];
        Assert.True(address.TryWriteTo(copied));
        Assert.Equal(expected, copied.ToArray());

        address.CopyTo(copied);
        Assert.Equal(expected, copied.ToArray());
        Assert.Equal(expected, address.ToArray());

        Span<byte> tooSmall = stackalloc byte[Address.BYTES_LENGTH - 1];
        Assert.False(address.TryWriteTo(tooSmall));
    }

    [Fact]
    public void Should_Format_With_Expected_Hex_Casing()
    {
        var address = Address.Parse("0x00112233445566778899AABBCCDDEEFF00112233");

        Assert.Equal("0x00112233445566778899aabbccddeeff00112233", address.ToString());
        Assert.Equal("00112233445566778899AABBCCDDEEFF00112233", address.ToHex());
    }

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
