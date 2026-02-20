using EtherSharp.ABI;
using EtherSharp.Numerics;
using System.Buffers.Binary;
using System.Text;

namespace EtherSharp.Tests.ABI.Decoder;

public class DecoderEdgeCaseTests
{
    [Fact]
    public void Should_Decode_Empty_Bytes()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        var result = new AbiDecoder(input).Bytes();
        Assert.Equal(0, result.Length);
    }

    [Fact]
    public void Should_Decode_Single_Byte()
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000011200000000000000000000000000000000000000000000000000000000000000");
        var result = new AbiDecoder(input).Bytes();
        Assert.Single(result.ToArray());
        Assert.Equal(0x12, result.Span[0]);
    }

    [Fact]
    public void Should_Decode_Empty_String()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        string result = new AbiDecoder(input).String();
        Assert.Equal("", result);
    }

    [Fact]
    public void Should_Decode_Empty_BoolArray()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        bool[] result = new AbiDecoder(input).BoolArray();
        Assert.Empty(result);
    }

    [Fact]
    public void Should_Decode_Empty_AddressArray()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        var result = new AbiDecoder(input).AddressArray();
        Assert.Empty(result);
    }

    [Fact]
    public void Should_Decode_Empty_StringArray()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        string[] result = new AbiDecoder(input).StringArray();
        Assert.Empty(result);
    }

    [Fact]
    public void Should_Decode_Empty_BytesArray()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        var result = new AbiDecoder(input).BytesArray();
        Assert.Empty(result);
    }

    [Fact]
    public void Should_Decode_Bool_True()
    {
        byte[] input = new byte[32];
        input[31] = 1;
        bool result = new AbiDecoder(input).Bool();
        Assert.True(result);
    }

    [Fact]
    public void Should_Decode_Bool_False()
    {
        byte[] input = new byte[32];
        bool result = new AbiDecoder(input).Bool();
        Assert.False(result);
    }

    [Fact]
    public void Should_Decode_Bool_False_When_AllOnes()
    {
        byte[] input = new byte[32];
        input.AsSpan().Fill(0xFF);
        input[31] = 0;
        bool result = new AbiDecoder(input).Bool();
        Assert.False(result);
    }

    [Fact]
    public void Should_Decode_Bool_False_When_LastByteIs2()
    {
        byte[] input = new byte[32];
        input[31] = 2;
        bool result = new AbiDecoder(input).Bool();
        Assert.False(result);
    }

    [Fact]
    public void Should_Decode_Bool_False_When_LastByteIs255()
    {
        byte[] input = new byte[32];
        input[31] = 255;
        bool result = new AbiDecoder(input).Bool();
        Assert.False(result);
    }

    [Fact]
    public void Should_Decode_Address_AllZeros()
    {
        byte[] input = new byte[32];
        var result = new AbiDecoder(input).Address();
        Assert.Equal(new byte[20], result.Bytes.ToArray());
    }

    [Fact]
    public void Should_Decode_Address_AllOnes()
    {
        byte[] input = new byte[32];
        input.AsSpan().Fill(0xFF);
        var result = new AbiDecoder(input).Address();
        byte[] expectedAddress = new byte[20];
        expectedAddress.AsSpan().Fill(0xFF);
        Assert.Equal(expectedAddress, result.Bytes.ToArray());
    }

    [Fact]
    public void Should_Throw_On_Invalid_SizedBytes_BitLength()
    {
        byte[] input = new byte[32];
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).SizedBytes(7));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).SizedBytes(0));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).SizedBytes(264));
    }

    [Fact]
    public void Should_Throw_On_Invalid_Number_BitLength()
    {
        byte[] input = new byte[32];
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<int>(true, 7));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<int>(true, 0));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<int>(true, 264));
    }

    [Fact]
    public void Should_Throw_On_Invalid_NumberArray_BitLength()
    {
        byte[] input = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).NumberArray<byte>(true, 7));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).NumberArray<byte>(true, 0));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).NumberArray<byte>(true, 264));
    }

    [Fact]
    public void Should_Throw_On_Wrong_Number_Type()
    {
        byte[] input = new byte[32];
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<byte>(false, 8));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<sbyte>(true, 8));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<int>(true, 16));
        Assert.Throws<ArgumentException>(() => new AbiDecoder(input).Number<uint>(false, 16));
    }

    [Fact]
    public void Should_Decode_Int256_MinValue()
    {
        byte[] input = new byte[32];
        input[0] = 128;
        var result = new AbiDecoder(input).Int256();
        Assert.Equal(Int256.MinValue, result);
    }

    [Fact]
    public void Should_Decode_Int256_MaxValue()
    {
        byte[] input = new byte[32];
        input.AsSpan()[1..].Fill(0xFF);
        input[0] = 0x7F;
        var result = new AbiDecoder(input).Int256();
        Assert.Equal(Int256.MaxValue, result);
    }

    [Fact]
    public void Should_Decode_UInt256_MaxValue()
    {
        byte[] input = new byte[32];
        input.AsSpan().Fill(0xFF);
        var result = new AbiDecoder(input).UInt256();
        Assert.Equal(UInt256.MaxValue, result);
    }

    [Fact]
    public void Should_Decode_SingleElement_Array()
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000001");
        var result = new AbiDecoder(input).Int256Array();
        Assert.Single(result);
        Assert.Equal(Int256.One, result[0]);
    }

    [Fact]
    public void Should_Decode_Bytes_Exactly32Bytes()
    {
        byte[] bytes = new byte[32];
        for(int i = 0; i < 32; i++)
        {
            bytes[i] = (byte) i;
        }

        byte[] input = new byte[96];
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[28..32], 32);
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[60..64], 32);
        bytes.CopyTo(input.AsSpan()[64..]);

        var result = new AbiDecoder(input).Bytes();
        Assert.Equal(32, result.Length);
        Assert.Equal(bytes, result.ToArray());
    }

    [Fact]
    public void Should_Decode_Bytes_33Bytes()
    {
        byte[] bytes = new byte[33];
        for(int i = 0; i < 33; i++)
        {
            bytes[i] = (byte) i;
        }

        byte[] input = new byte[128];
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[28..32], 32);
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[60..64], 33);
        bytes.CopyTo(input.AsSpan()[64..]);

        var result = new AbiDecoder(input).Bytes();
        Assert.Equal(33, result.Length);
        Assert.Equal(bytes, result.ToArray());
    }

    [Fact]
    public void Should_Decode_Large_String()
    {
        string largeString = new string('a', 1000);
        int encodedLength = Encoding.UTF8.GetByteCount(largeString);

        byte[] stringBytes = Encoding.UTF8.GetBytes(largeString);
        int paddedLength = ((encodedLength / 32) + 1) * 32;

        byte[] input = new byte[64 + paddedLength];
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[28..32], 32);
        BinaryPrimitives.WriteUInt32BigEndian(input.AsSpan()[60..64], (uint) encodedLength);
        stringBytes.CopyTo(input.AsSpan()[64..]);

        string result = new AbiDecoder(input).String();
        Assert.Equal(largeString, result);
    }
}
