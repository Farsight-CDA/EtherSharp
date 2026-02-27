using EtherSharp.Common;
using EtherSharp.Types;
using System.Text.Json;

namespace EtherSharp.Tests.Types;

public class Bytes32Tests
{
    private const string SAMPLE_BYTES32 = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff";

    [Fact]
    public void Should_Parse_From_Prefixed_Hex()
    {
        var bytes32 = Bytes32.Parse(SAMPLE_BYTES32);
        Assert.Equal(SAMPLE_BYTES32, bytes32.ToString(), ignoreCase: true);
        Assert.Equal(Bytes32.BYTE_LENGTH, bytes32.Bytes.Length);
    }

    [Fact]
    public void Should_Parse_From_Unprefixed_Hex()
    {
        var bytes32 = Bytes32.Parse(SAMPLE_BYTES32[2..]);
        Assert.Equal(SAMPLE_BYTES32, bytes32.ToString(), ignoreCase: true);
    }

    [Fact]
    public void Should_Parse_From_CharSpan()
    {
        var span = SAMPLE_BYTES32.AsSpan();
        var bytes32 = Bytes32.Parse(span);

        Assert.Equal(SAMPLE_BYTES32, bytes32.ToString(), ignoreCase: true);
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
        bool parsed = Bytes32.TryParse(value, out var bytes32);
        Assert.False(parsed);
        Assert.Equal(Bytes32.Zero, bytes32);
    }

    [Fact]
    public void Should_TryParse_From_CharSpan()
    {
        bool parsed = Bytes32.TryParse(SAMPLE_BYTES32.AsSpan(), out var bytes32);
        Assert.True(parsed);
        Assert.Equal(SAMPLE_BYTES32, bytes32.ToString(), ignoreCase: true);
    }

    [Fact]
    public void Should_Throw_On_Invalid_Parse()
        => Assert.Throws<FormatException>(() => Bytes32.Parse("0x1234"));

    [Fact]
    public void Should_Create_From_Bytes_And_Return_Copy()
    {
        byte[] bytes = Convert.FromHexString(SAMPLE_BYTES32[2..]);
        var bytes32 = Bytes32.FromBytes(bytes);
        byte[] copy = bytes32.ToArray();

        Assert.Equal(bytes, copy);
        copy[0] ^= 0xFF;
        Assert.NotEqual(copy, bytes32.ToArray());
    }

    [Fact]
    public void Should_Throw_When_FromBytes_Length_Is_Not_32()
        => Assert.Throws<ArgumentException>(() => Bytes32.FromBytes(new byte[31]));

    [Fact]
    public void Should_Compare_And_Order_By_BigEndian_Byte_Order()
    {
        var low = Bytes32.Parse("0x0000000000000000000000000000000000000000000000000000000000000001");
        var high = Bytes32.Parse("0x0000000000000000000000000000000000000000000000000000000000000002");
        var muchHigher = Bytes32.Parse("0x0100000000000000000000000000000000000000000000000000000000000000");

        Assert.True(low.CompareTo(high) < 0);
        Assert.True(high.CompareTo(low) > 0);
        Assert.True(muchHigher.CompareTo(high) > 0);
        Assert.Equal(0, low.CompareTo(low));
    }

    [Fact]
    public void Should_Serialize_And_Deserialize_Using_Default_Json()
    {
        var bytes32 = Bytes32.Parse(SAMPLE_BYTES32);
        string json = JsonSerializer.Serialize(bytes32);
        var roundtrip = JsonSerializer.Deserialize<Bytes32>(json);

        Assert.Equal($"\"{SAMPLE_BYTES32}\"", json, ignoreCase: true);
        Assert.Equal(bytes32, roundtrip);
    }

    [Fact]
    public void Should_Serialize_And_Deserialize_Using_Evm_Options()
    {
        var bytes32 = Bytes32.Parse(SAMPLE_BYTES32);
        string json = JsonSerializer.Serialize(bytes32, ParsingUtils.EvmSerializerOptions);
        var roundtrip = JsonSerializer.Deserialize<Bytes32>(json, ParsingUtils.EvmSerializerOptions);

        Assert.Equal($"\"{SAMPLE_BYTES32}\"", json, ignoreCase: true);
        Assert.Equal(bytes32, roundtrip);
    }
}
