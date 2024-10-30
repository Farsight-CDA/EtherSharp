using EtherSharp.ABI;

namespace EtherSharp.Tests;

public class UnitDynamicTypesAbiEncoder
{
    private readonly AbiEncoder _encoder;

    public UnitDynamicTypesAbiEncoder()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Test_String()
    {
        string bigIntValue = "hello_world";
        string @string = "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
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
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_int8()
    {
        string @string = "0000000000000000000000000000000000000000000000000000000000000001";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Int8(1);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_int_String()
    {
        string bigIntValue = "hello_worldwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww";
        string @string = "0000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000c468656c6c6f5f776f726c64777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777700000000000000000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = _encoder.Int8(1).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(stringByte, actualOutput);
    }

    [Fact]
    public void Test_Array_with_int8()
    {

        int[] arr = [1, 2, 3, 4];
        _ = _encoder.Array(x => x.Struct(1, y => y.Int8(8)));

        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(new byte[2], actualOutput);
    }
}