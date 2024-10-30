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
        string String = "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] StringByte = Convert.FromHexString(String);

        _ = _encoder.String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(StringByte, actualOutput);
    }

    [Fact]
    public void Test_2String()
    {
        string bigIntValue = "hello_world";
        string String = "00000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] StringByte = Convert.FromHexString(String);

        _ = _encoder.String(bigIntValue).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(StringByte, actualOutput);
    }

    [Fact]
    public void Test_int8()
    {
        string String = "0000000000000000000000000000000000000000000000000000000000000001";

        byte[] StringByte = Convert.FromHexString(String);

        _ = _encoder.Int8(1);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(StringByte, actualOutput);
    }

    [Fact]
    public void Test_int_String()
    {
        string bigIntValue = "hello_worldwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww";
        string String = "0000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000c468656c6c6f5f776f726c64777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777700000000000000000000000000000000000000000000000000000000";

        byte[] StringByte = Convert.FromHexString(String);

        _ = _encoder.Int8(1).String(bigIntValue);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(StringByte, actualOutput);
    }

    [Fact]
    public void Test_Array_with_int8()
    {
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        //Assert.Equal(StringByte, actualOutput);
    }
}