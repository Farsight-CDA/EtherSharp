using EtherSharp.Common;
using EtherSharp.Types;
using System.Text.Json;

namespace EtherSharp.Tests.Types;

public class Hash32Tests
{
    private const string SAMPLE_HASH = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff";

    [Fact]
    public void Should_Parse_From_Prefixed_Hex()
    {
        var hash = Hash32.Parse(SAMPLE_HASH);
        Assert.Equal(SAMPLE_HASH, hash.ToString(), ignoreCase: true);
        Assert.Equal(Hash32.BYTE_LENGTH, hash.Bytes.Length);
    }

    [Fact]
    public void Should_Parse_From_Unprefixed_Hex()
    {
        var hash = Hash32.Parse(SAMPLE_HASH[2..]);
        Assert.Equal(SAMPLE_HASH, hash.ToString(), ignoreCase: true);
    }

    [Fact]
    public void Should_Parse_From_CharSpan()
    {
        var span = SAMPLE_HASH.AsSpan();
        var hash = Hash32.Parse(span);

        Assert.Equal(SAMPLE_HASH, hash.ToString(), ignoreCase: true);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0x")]
    [InlineData("0x1")]
    [InlineData("123")]
    [InlineData("0x1234")]
    [InlineData("1234")]
    [InlineData("0x00112233445566778899aabbccddeeff00112233445566778899aabbccddeezz")]
    public void Should_Return_False_On_Invalid_TryParse(string? value)
    {
        bool parsed = Hash32.TryParse(value, out var hash);
        Assert.False(parsed);
        Assert.Equal(Hash32.Zero, hash);
    }

    [Fact]
    public void Should_TryParse_From_CharSpan()
    {
        bool parsed = Hash32.TryParse(SAMPLE_HASH.AsSpan(), out var hash);
        Assert.True(parsed);
        Assert.Equal(SAMPLE_HASH, hash.ToString(), ignoreCase: true);
    }

    [Fact]
    public void Should_Throw_On_Invalid_Parse()
        => Assert.Throws<FormatException>(() => Hash32.Parse("0x1234"));

    [Fact]
    public void Should_Create_From_Bytes_And_Return_Copy()
    {
        byte[] bytes = Convert.FromHexString(SAMPLE_HASH[2..]);
        var hash = Hash32.FromBytes(bytes);
        byte[] copy = hash.ToArray();

        Assert.Equal(bytes, copy);
        copy[0] ^= 0xFF;
        Assert.NotEqual(copy, hash.ToArray());
    }

    [Fact]
    public void Should_Throw_When_FromBytes_Length_Is_Not_32()
        => Assert.Throws<ArgumentException>(() => Hash32.FromBytes(new byte[31]));

    [Fact]
    public void Should_Compare_And_Order_By_BigEndian_Byte_Order()
    {
        var low = Hash32.Parse("0x0000000000000000000000000000000000000000000000000000000000000001");
        var high = Hash32.Parse("0x0000000000000000000000000000000000000000000000000000000000000002");
        var muchHigher = Hash32.Parse("0x0100000000000000000000000000000000000000000000000000000000000000");

        Assert.True(low.CompareTo(high) < 0);
        Assert.True(high.CompareTo(low) > 0);
        Assert.True(muchHigher.CompareTo(high) > 0);
        Assert.Equal(0, low.CompareTo(low));
    }

    [Fact]
    public void Should_Serialize_And_Deserialize_Using_Default_Json()
    {
        var hash = Hash32.Parse(SAMPLE_HASH);
        string json = JsonSerializer.Serialize(hash);
        var roundtrip = JsonSerializer.Deserialize<Hash32>(json);

        Assert.Equal($"\"{SAMPLE_HASH}\"", json, ignoreCase: true);
        Assert.Equal(hash, roundtrip);
    }

    [Fact]
    public void Should_Serialize_And_Deserialize_Using_Evm_Options()
    {
        var hash = Hash32.Parse(SAMPLE_HASH);
        string json = JsonSerializer.Serialize(hash, ParsingUtils.EvmSerializerOptions);
        var roundtrip = JsonSerializer.Deserialize<Hash32>(json, ParsingUtils.EvmSerializerOptions);

        Assert.Equal($"\"{SAMPLE_HASH}\"", json, ignoreCase: true);
        Assert.Equal(hash, roundtrip);
    }
}
