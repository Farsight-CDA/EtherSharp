﻿using EtherSharp.ABI;
using EtherSharp.Contract;

namespace EtherSharp.Tests.ABI.Encoder;
public class OtherAbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public OtherAbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Nested_Number_Arrays()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000a000000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002");
        byte[] actual = _encoder.Array(x => x.Int256Array(0).Int256Array(1).Int256Array(2)).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multi_Nested_Number_Arrays()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000012000000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.Array(x =>
            x.Array(y =>
                y.Int256Array(1, 2, 3)
                    .Int256Array(10)
                    .Int256Array())
            ).Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_StringArray()
    {
        byte[] expected = Convert.FromHexString("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000548656c6c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005576f726c64000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder.StringArray("Hello", "World").Build();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_2String()
    {
        string bigIntValue = "hello_world";
        string @string = "00000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.String(bigIntValue).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.TryWritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_Struct()
    {
        string @string = "00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Struct(8, x => x.Int8(2).Int8(8));
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.TryWritoTo(actualOutput.AsSpan());
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
        _encoder.TryWritoTo(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }
}
