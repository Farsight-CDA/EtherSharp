using EtherSharp.Crypto;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Text;

namespace EtherSharp.Tests.EIPs;

public sealed class EIP191Tests
{
    [Theory]
    [InlineData("", "5f35dce98ba4fba25530a026ed80b2cecdaa31091ba4958b99b52ea1d068adad")]
    [InlineData("hello", "50b2c43fd39106bafbba0da34fc430e1f91e3c96ea2acee2bc34119f92b37750")]
    [InlineData("é🙂", "0e7dbed7ca8c1c55d1fe8a6ae7b67c2b94ebb01493fc427e159993c2fb2d1472")]
    public void HashPersonalMessage_Uses_Utf8_Byte_Length(string message, string expected)
    {
        Assert.Equal(expected, Convert.ToHexString(EIP191.HashPersonalMessage(message).ToArray()), ignoreCase: true);
        Assert.Equal(expected, Convert.ToHexString(EIP191.HashPersonalMessage(Encoding.UTF8.GetBytes(message)).ToArray()), ignoreCase: true);
    }

    [Fact]
    public void HashPersonalMessage_Handles_Large_Messages()
    {
        string message = new('a', 1000);

        Assert.Equal(
            "646dfe80977f3cb244f566d96cd3aabb891d47b9ba5159076d78e9999835e0d6",
            Convert.ToHexString(EIP191.HashPersonalMessage(Encoding.ASCII.GetBytes(message)).ToArray()),
            ignoreCase: true
        );
        Assert.Equal(
            "646dfe80977f3cb244f566d96cd3aabb891d47b9ba5159076d78e9999835e0d6",
            Convert.ToHexString(EIP191.HashPersonalMessage(message).ToArray()),
            ignoreCase: true
        );
    }

    [Fact]
    public void HashIntendedValidator_Includes_Address_And_Raw_Data()
    {
        var validator = Address.FromString("0x1234567890123456789012345678901234567890");

        Assert.Equal(
            "f49b23405a95adab11b4162b87dbcdcf886348f14445e286cfdd3ee39d8a2f68",
            Convert.ToHexString(EIP191.HashIntendedValidator(validator, "hello"u8).ToArray()),
            ignoreCase: true
        );
    }
}
