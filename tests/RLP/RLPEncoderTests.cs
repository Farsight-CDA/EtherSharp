using EtherSharp.Numerics;
using EtherSharp.RLP;
using System.Text;

namespace EtherSharp.Tests.RLP;

public class RLPEncoderTests
{
    [Fact]
    public void Should_Encode_Empty_String()
    {
        Span<byte> buffer = stackalloc byte[RLPEncoder.GetStringSize([])];

        _ = new RLPEncoder(buffer).EncodeString();

        Assert.Equal(Convert.FromHexString("80"), buffer.ToArray());
    }

    [Fact]
    public void Should_Encode_Single_Byte_String_Without_Prefix()
    {
        Span<byte> buffer = stackalloc byte[RLPEncoder.GetStringSize([0x7f])];

        _ = new RLPEncoder(buffer).EncodeString(0x7f);

        Assert.Equal(Convert.FromHexString("7f"), buffer.ToArray());
    }

    [Fact]
    public void Should_Encode_Known_String_Vector()
    {
        byte[] dog = Encoding.ASCII.GetBytes("dog");
        Span<byte> buffer = stackalloc byte[RLPEncoder.GetStringSize(dog)];

        _ = new RLPEncoder(buffer).EncodeString(dog);

        Assert.Equal(Convert.FromHexString("83646f67"), buffer.ToArray());
    }

    [Fact]
    public void Should_Encode_Known_List_Vector()
    {
        byte[] cat = Encoding.ASCII.GetBytes("cat");
        byte[] dog = Encoding.ASCII.GetBytes("dog");

        int contentSize = RLPEncoder.GetStringSize(cat) + RLPEncoder.GetStringSize(dog);
        byte[] buffer = new byte[RLPEncoder.GetListSize(contentSize)];

        _ = new RLPEncoder(buffer)
            .EncodeList(contentSize)
            .EncodeString(cat)
            .EncodeString(dog);

        Assert.Equal(Convert.FromHexString("c88363617483646f67"), buffer);
    }

    [Fact]
    public void Should_Encode_Empty_List()
    {
        Span<byte> buffer = stackalloc byte[RLPEncoder.GetListSize(0)];

        _ = new RLPEncoder(buffer).EncodeList(0);

        Assert.Equal(Convert.FromHexString("c0"), buffer.ToArray());
    }

    [Fact]
    public void Should_Encode_Single_Byte_0x80_With_String_Prefix()
    {
        Span<byte> buffer = stackalloc byte[RLPEncoder.GetStringSize([0x80])];

        _ = new RLPEncoder(buffer).EncodeString(0x80);

        Assert.Equal(Convert.FromHexString("8180"), buffer.ToArray());
    }

    [Fact]
    public void Should_Encode_Short_String_With_Length_Of_55_Bytes()
    {
        byte[] payload = new byte[55];
        payload.AsSpan().Fill(0x61);
        byte[] buffer = new byte[RLPEncoder.GetStringSize(payload)];

        _ = new RLPEncoder(buffer).EncodeString(payload);

        Assert.Equal(0xb7, buffer[0]);
        Assert.Equal(payload, buffer[1..]);
    }

    [Fact]
    public void Should_Encode_Long_String_With_Length_Of_56_Bytes()
    {
        byte[] payload = new byte[56];
        payload.AsSpan().Fill(0x61);
        byte[] buffer = new byte[RLPEncoder.GetStringSize(payload)];

        _ = new RLPEncoder(buffer).EncodeString(payload);

        Assert.Equal(0xb8, buffer[0]);
        Assert.Equal(56, buffer[1]);
        Assert.Equal(payload, buffer[2..]);
    }

    [Fact]
    public void Should_Encode_Long_List_Prefix_When_Content_Exceeds_55_Bytes()
    {
        byte[] item = new byte[20];
        item.AsSpan().Fill(0x01);
        int contentSize = RLPEncoder.GetStringSize(item) * 3;
        byte[] buffer = new byte[RLPEncoder.GetListSize(contentSize)];

        _ = new RLPEncoder(buffer)
            .EncodeList(contentSize)
            .EncodeString(item)
            .EncodeString(item)
            .EncodeString(item);

        Assert.Equal(0xf8, buffer[0]);
        Assert.Equal(contentSize, buffer[1]);
    }

    [Fact]
    public void Should_Encode_Short_List_Prefix_When_Content_Is_55_Bytes()
    {
        byte[] item = new byte[54];
        item.AsSpan().Fill(0x01);

        int contentSize = RLPEncoder.GetStringSize(item);
        byte[] buffer = new byte[RLPEncoder.GetListSize(contentSize)];

        _ = new RLPEncoder(buffer)
            .EncodeList(contentSize)
            .EncodeString(item);

        Assert.Equal(0xf7, buffer[0]);
    }

    [Fact]
    public void Should_Encode_Long_String_Prefix_With_Two_Length_Bytes()
    {
        byte[] payload = new byte[256];
        payload.AsSpan().Fill(0x61);
        byte[] buffer = new byte[RLPEncoder.GetStringSize(payload)];

        _ = new RLPEncoder(buffer).EncodeString(payload);

        Assert.Equal(0xb9, buffer[0]);
        Assert.Equal(0x01, buffer[1]);
        Assert.Equal(0x00, buffer[2]);
        Assert.Equal(payload, buffer[3..]);
    }

    [Fact]
    public void Should_Encode_Long_List_Prefix_With_Two_Length_Bytes()
    {
        byte[] item = new byte[20];
        item.AsSpan().Fill(0x01);

        int contentSize = RLPEncoder.GetStringSize(item) * 13;
        byte[] buffer = new byte[RLPEncoder.GetListSize(contentSize)];

        var encoder = new RLPEncoder(buffer).EncodeList(contentSize);
        for(int i = 0; i < 13; i++)
        {
            encoder = encoder.EncodeString(item);
        }

        Assert.Equal(0xf9, buffer[0]);
        Assert.Equal(0x01, buffer[1]);
        Assert.Equal(0x11, buffer[2]);
    }

    [Theory]
    [InlineData((uint) 0, "80")]
    [InlineData((uint) 15, "0f")]
    [InlineData((uint) 127, "7f")]
    [InlineData((uint) 128, "8180")]
    [InlineData((uint) 1024, "820400")]
    public void Should_Encode_UInt32_Values(uint value, string expected)
    {
        byte[] buffer = new byte[RLPEncoder.GetIntSize(value)];

        _ = new RLPEncoder(buffer).EncodeInt(value);

        Assert.Equal(Convert.FromHexString(expected), buffer);
    }

    [Theory]
    [InlineData((ulong) 0, "80")]
    [InlineData((ulong) 127, "7f")]
    [InlineData((ulong) 128, "8180")]
    [InlineData((ulong) 255, "81ff")]
    [InlineData((ulong) 256, "820100")]
    [InlineData((ulong) 4294967296, "850100000000")]
    [InlineData(UInt64.MaxValue, "88ffffffffffffffff")]
    public void Should_Encode_UInt64_Values(ulong value, string expected)
    {
        byte[] buffer = new byte[RLPEncoder.GetIntSize(value)];

        _ = new RLPEncoder(buffer).EncodeInt(value);

        Assert.Equal(Convert.FromHexString(expected), buffer);
    }

    [Theory]
    [InlineData("00", "80")]
    [InlineData("7f", "7f")]
    [InlineData("80", "8180")]
    [InlineData("0102030405060708090a0b0c0d0e0f10", "900102030405060708090a0b0c0d0e0f10")]
    [InlineData("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", "a0ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")]
    public void Should_Encode_UInt256_Values(string valueHex, string expected)
    {
        Assert.True(UInt256.TryParseFromHex(valueHex, out var value));

        byte[] actual = new byte[RLPEncoder.GetIntSize(value)];
        _ = new RLPEncoder(actual).EncodeInt(value);

        Assert.Equal(Convert.FromHexString(expected), actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(55)]
    [InlineData(56)]
    [InlineData(1024)]
    public void Size_Utilities_Should_Advance_Encoder_Cursor_Correctly(int length)
    {
        byte[] data = new byte[length];
        int stringSize = RLPEncoder.GetStringSize(data);

        byte[] stringBuffer = new byte[stringSize + 1];
        stringBuffer.AsSpan().Fill(0xaa);
        _ = new RLPEncoder(stringBuffer)
            .EncodeString(data)
            .EncodeString((byte) 0x01);

        Assert.Equal(0x01, stringBuffer[stringSize]);

        int listContentSize = stringSize;
        int listSize = RLPEncoder.GetListSize(listContentSize);
        byte[] listBuffer = new byte[listSize + 1];
        listBuffer.AsSpan().Fill(0xaa);
        _ = new RLPEncoder(listBuffer)
            .EncodeList(listContentSize)
            .EncodeString(data)
            .EncodeString((byte) 0x01);

        Assert.Equal(0x01, listBuffer[listSize]);
    }
}
