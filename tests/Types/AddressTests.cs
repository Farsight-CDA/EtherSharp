using EtherSharp.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Tests.Types;

public sealed class AddressTests
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

    [Theory]
    [InlineData(0, "0xbd770416a3345f91e4b34576cb804a576fa48eb1")]
    [InlineData(1, "0x5a443704dd4b594b382c22a083e2bd3090a6fef3")]
    [InlineData(127, "0x5a1bfc20f2037f3e54d367a70957a5327130cea5")]
    [InlineData(128, "0xc1784bd8a0ffebd60d0bc7099dcd811b57f30bc4")]
    [InlineData(256, "0x1183a5a83c1fa113618603abc4509077ec672699")]
    [InlineData(UInt64.MaxValue, "0x1262d73ea59d3a661bf8751d16cf1a5377149e75")]
    public void Should_Derive_Create_Address(ulong oldNonce, string expected)
    {
        var creator = Address.Zero;

        var actual = Address.DeriveCreate(in creator, oldNonce);

        Assert.Equal(Address.Parse(expected), actual);
    }

    [Theory]
    [InlineData(
        "0x0000000000000000000000000000000000000000",
        "0x0000000000000000000000000000000000000000000000000000000000000000",
        "00",
        "0x4D1A2e2bB4F88F0250f26Ffff098B0b30B26BF38")
    ]
    [InlineData(
        "0xdeadbeef00000000000000000000000000000000",
        "0x0000000000000000000000000000000000000000000000000000000000000000",
        "00",
        "0xB928f69Bb1D91Cd65274e3c79d8986362984fDA3")
    ]
    [InlineData(
        "0xdeadbeef00000000000000000000000000000000",
        "0x000000000000000000000000feed000000000000000000000000000000000000",
        "00",
        "0xD04116cDd17beBE565EB2422F2497E06cC1C9833")
    ]
    [InlineData(
        "0x0000000000000000000000000000000000000000",
        "0x0000000000000000000000000000000000000000000000000000000000000000",
        "deadbeef",
        "0x70f2b2914A2a4b783FaEFb75f459A580616Fcb5e")
    ]
    [InlineData(
        "0x0000000000000000000000000000000000000000",
        "0x0000000000000000000000000000000000000000000000000000000000000000",
        "",
        "0xE33C0C7F7df4809055C3ebA6c09CFe4BaF1BD9e0")
    ]
    public void Should_Derive_Create2_Address(
        string creator,
        string salt,
        string initCode,
        string expected)
    {
        var creatorAddress = Address.Parse(creator);
        var saltBytes = Bytes32.Parse(salt);
        byte[] initCodeBytes = Convert.FromHexString(initCode);
        var initCodeHash = Keccak256.HashData(initCodeBytes);

        var actual = Address.DeriveCreate2(in creatorAddress, in saltBytes, in initCodeHash);

        var expectedAddress = Address.Parse(expected);
        Assert.Equal(expectedAddress, actual);
    }
}
