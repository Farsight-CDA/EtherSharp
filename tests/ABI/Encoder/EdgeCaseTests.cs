using EtherSharp.ABI;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace EtherSharp.Tests.ABI.Encoder;

public class EdgeCaseTests
{
    private readonly AbiEncoder _encoder;

    public EdgeCaseTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Should_Encode_Empty_Bytes()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Bytes(ReadOnlyMemory<byte>.Empty).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Single_Byte()
    {
        byte[] expected = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000011200000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Bytes(new byte[] { 0x12 }).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Bytes_Exactly_32_Bytes()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Create().GetBytes(bytes);

        byte[] actual = _encoder.Bytes(bytes).Build();

        uint lengthValue = BinaryPrimitives.ReadUInt32BigEndian(actual.AsSpan()[28..32]);
        Assert.Equal(32u, lengthValue);
        Assert.Equal(bytes, actual.AsSpan()[64..96].ToArray());
    }

    [Fact]
    public void Should_Encode_Bytes_33_Bytes()
    {
        byte[] bytes = new byte[33];
        RandomNumberGenerator.Create().GetBytes(bytes);

        byte[] actual = _encoder.Bytes(bytes).Build();

        uint lengthValue = BinaryPrimitives.ReadUInt32BigEndian(actual.AsSpan()[60..64]);
        Assert.Equal(33u, lengthValue);
        Assert.Equal(bytes, actual.AsSpan()[64..97].ToArray());
    }

    [Fact]
    public void Should_Encode_Empty_String()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.String("").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Empty_StringArray()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.StringArray([]).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Empty_BoolArray()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.BoolArray([]).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Empty_AddressArray()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.AddressArray([]).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Empty_BytesArray()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.BytesArray([]).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Bool_True()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000001");
        byte[] actual = _encoder.Bool(true).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Bool_False()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Bool(false).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Address_AllZeros()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Address(Address.FromBytes(new byte[20])).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Address_AllOnes()
    {
        byte[] addressBytes = new byte[20];
        addressBytes.AsSpan().Fill(0xFF);
        byte[] expected = Convert.FromHexString("000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
        byte[] actual = _encoder.Address(Address.FromBytes(addressBytes)).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Int256_MinValue()
    {
        byte[] expected = new byte[32];
        expected[0] = 128;
        byte[] actual = _encoder.Int256(Int256.MinValue).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Int256_MaxValue()
    {
        byte[] expected = new byte[32];
        expected.AsSpan()[1..].Fill(0xFF);
        expected[0] = 0x7F;
        byte[] actual = _encoder.Int256(Int256.MaxValue).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_UInt256_MaxValue()
    {
        byte[] expected = new byte[32];
        expected.AsSpan().Fill(0xFF);
        byte[] actual = _encoder.UInt256(UInt256.MaxValue).Build();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    public void Should_Encode_Empty_NumberArray(int bitSize)
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = bitSize switch
        {
            8 => _encoder.Int8Array([]).Build(),
            16 => _encoder.Int16Array([]).Build(),
            32 => _encoder.Int32Array([]).Build(),
            64 => _encoder.Int64Array([]).Build(),
            128 => _encoder.Int128Array([]).Build(),
            256 => _encoder.Int256Array([]).Build(),
            _ => throw new NotSupportedException()
        };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Encode_Bytes1_AllOnes()
    {
        byte[] actual = _encoder.Bytes1(0xFF).Build();
        Assert.Equal(0xFF, actual[31]);
    }

    [Fact]
    public void Should_Encode_Bytes32_AllOnes()
    {
        byte[] bytes = new byte[32];
        bytes.AsSpan().Fill(0xFF);
        byte[] actual = _encoder.Bytes32(bytes).Build();
        Assert.Equal(0xFF, actual[31]);
        Assert.Equal(0xFF, actual[0]);
    }

    [Fact]
    public void Should_Encode_SingleElement_Array()
    {
        byte[] actual = _encoder.Int256Array([Int256.One]).Build();
        Assert.Equal(96, actual.Length);
    }

    [Fact]
    public void Should_Encode_Large_String()
    {
        string largeString = new string('a', 1000);
        byte[] actual = _encoder.String(largeString).Build();
        uint lengthValue = BinaryPrimitives.ReadUInt32BigEndian(actual.AsSpan()[60..64]);
        Assert.Equal(1000u, lengthValue);
    }

    [Fact]
    public void Should_Encode_FixedTuple_With_MixedTypes()
    {
        byte[] actual = _encoder.FixedTuple(e => e.Bool(true).Address(Address.FromBytes(new byte[20]))).Build();
        Assert.Equal(64, actual.Length);
    }

    [Fact]
    public void Should_Throw_On_Invalid_BitLength_Number()
    {
        Assert.Throws<ArgumentException>(() => _encoder.Number(0, true, 7));
        Assert.Throws<ArgumentException>(() => _encoder.Number(0, true, 0));
        Assert.Throws<ArgumentException>(() => _encoder.Number(0, true, 264));
        Assert.Throws<ArgumentException>(() => _encoder.Number(0, true, 33));
    }

    [Fact]
    public void Should_Throw_On_Invalid_BitLength_NumberArray()
    {
        Assert.Throws<ArgumentException>(() => _encoder.NumberArray<byte>(true, 7, []));
        Assert.Throws<ArgumentException>(() => _encoder.NumberArray<byte>(true, 0, []));
        Assert.Throws<ArgumentException>(() => _encoder.NumberArray<byte>(true, 264, []));
    }

    [Fact]
    public void Should_Throw_On_Empty_DynamicTuple()
        => Assert.Throws<InvalidOperationException>(() => _encoder.DynamicTuple(_ => { }).Build());

    [Fact]
    public void Should_Encode_DynamicTuple_With_DynamicElement()
    {
        byte[] actual = _encoder.DynamicTuple(e => e.String("test")).Build();
        Assert.True(actual.Length > 64);
    }
}
