using System.Numerics;

namespace EVM.Net.Tests;

public class UnitMinMaxAbiEncoder
{
    private readonly AbiEncoder _encoder;

    public UnitMinMaxAbiEncoder()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Test_Int8_Min()
    {
        sbyte value = sbyte.MinValue; // -128
        string hexString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff80";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int8(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int8_Max()
    {
        sbyte value = sbyte.MaxValue; // 127
        string hexString = "000000000000000000000000000000000000000000000000000000000000007f";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int8(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt8_Min()
    {
        byte value = byte.MinValue; // 0
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt8(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt8_Max()
    {
        byte value = byte.MaxValue; // 255
        string hexString = "00000000000000000000000000000000000000000000000000000000000000ff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt8(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int16_Min()
    {
        short value = short.MinValue; // -32768
        string hexString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int16(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int16_Max()
    {
        short value = short.MaxValue; // 32767
        string hexString = "0000000000000000000000000000000000000000000000000000000000007fff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int16(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt16_Min()
    {
        ushort value = ushort.MinValue; // 0
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt16(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt16_Max()
    {
        ushort value = ushort.MaxValue; // 65535
        string hexString = "000000000000000000000000000000000000000000000000000000000000ffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt16(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int32_Min()
    {
        int value = int.MinValue; // -2147483648
        string hexString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffff80000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int32(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int32_Max()
    {
        int value = int.MaxValue; // 2147483647
        string hexString = "000000000000000000000000000000000000000000000000000000007fffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int32(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt32_Min()
    {
        uint value = uint.MinValue; // 0
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt32(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt32_Max()
    {
        uint value = uint.MaxValue; // 4294967295
        string hexString = "00000000000000000000000000000000000000000000000000000000ffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt32(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int64_Min()
    {
        long value = long.MinValue; // -9223372036854775808
        string hexString = "ffffffffffffffffffffffffffffffffffffffffffffffff8000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int64(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int64_Max()
    {
        long value = long.MaxValue; // 9223372036854775807
        string hexString = "0000000000000000000000000000000000000000000000007fffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int64(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt64_Min()
    {
        ulong value = ulong.MinValue; // 0
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt64(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt64_Max()
    {
        ulong value = ulong.MaxValue; // 18446744073709551615
        string hexString = "000000000000000000000000000000000000000000000000ffffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt64(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int128_Min()
    {
        var value = -BigInteger.Pow(2, 127);
        string hexString = "ffffffffffffffffffffffffffffffff80000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int128(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int128_Max()
    {
        var value = BigInteger.Parse("170141183460469231731687303715884105727");
        string hexString = "000000000000000000000000000000007fffffffffffffffffffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int128(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt128_Min()
    {
        var value = BigInteger.Parse("0");
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt128(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt128_Max()
    {
        var value = BigInteger.Parse("340282366920938463463374607431768211455");
        string hexString = "00000000000000000000000000000000ffffffffffffffffffffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt128(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int256_Min()
    {
        var value = -BigInteger.Pow(2, 255);
        string hexString = "8000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int256(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_Int256_Max()
    {
        var value = BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819967");
        string hexString = "7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.Int256(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt256_Min()
    {
        var value = BigInteger.Parse("0");
        string hexString = "0000000000000000000000000000000000000000000000000000000000000000";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt256(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }

    [Fact]
    public void Test_UInt256_Max()
    {
        var value = BigInteger.Pow(2, 256) - 1;
        string hexString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

        byte[] expectedBytes = Convert.FromHexString(hexString);

        _ = _encoder.UInt256(value);
        byte[] actualOutput = new byte[_encoder.Size];
        _encoder.Build(actualOutput.AsSpan());
        Assert.Equal(expectedBytes, actualOutput);
    }
}