﻿using EtherSharp.ABI;

namespace EtherSharp.Tests;
public class AbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public AbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Test_StringArray()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000548656c6c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005576f726c64000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.StringArray("Hello", "World").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_String()
    {
        string bigIntValue = "hello_world";
        string @string = "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.WritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_2String()
    {
        string bigIntValue = "hello_world";
        string @string = "00000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.String(bigIntValue).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.WritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_Struct()
    {
        string @string = "00000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Struct(8, x => x.Int8(2).Int8(8));
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.WritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_int8()
    {
        string @string = "0000000000000000000000000000000000000000000000000000000000000001";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Int8(1);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.WritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_int_String()
    {

        string bigIntValue = "hello_world";
        string @string = "00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000040000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Int8(1).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.WritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }
}