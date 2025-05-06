using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.E2E;
public class ComplexEncodingTests
{
    [Fact]
    public void Should_RoundTrip_Multiple_Numbers()
    {
        byte[] encoded = new AbiEncoder()
            .UInt8(byte.MaxValue)
            .UInt16(byte.MaxValue)
            .UInt24(50)
            .UInt32(byte.MinValue)
            .UInt40(byte.MinValue)
            .Build();

        var decoder = new AbiDecoder(encoded);
        byte a = decoder.UInt8();
        ushort b = decoder.UInt16();
        uint c = decoder.UInt24();
        uint d = decoder.UInt32();
        ulong e = decoder.UInt40();

        Assert.Equal(byte.MaxValue, a);
        Assert.Equal(byte.MaxValue, b);
        Assert.Equal((uint) 50, c);
        Assert.Equal(byte.MinValue, d);
        Assert.Equal(byte.MinValue, e);
    }

    [Fact]
    public void Should_RoundTrip_Nested_Int32_Array()
    {
        int[][][] input = [[[1, 2, 3, 4, 5]]];
        byte[] encoded = new AbiEncoder()
            .Array(input, (encoder, value) =>
                encoder.Array(value, (encoder, value) =>
                    encoder.Int32Array(value)))
            .Build();
        int[][][] output = new AbiDecoder(encoded)
            .Array(x =>
                x.Array(y =>
                    y.Int32Array()));

        Assert.Equal(input, output);
    }

    [Fact]
    public void Should_RoundTrip_Multiple_DynamicTypes()
    {
        byte[] encoded = new AbiEncoder()
            .Int32(16)
            .Int8Array(sbyte.MinValue, 0, sbyte.MaxValue)
            .String("Hello")
            .Array<byte[]>([[12]], (encoder, value) => encoder.Int8Array(12))
            .Build();

        var decoder = new AbiDecoder(encoded);

        int val1 = decoder.Int32();
        sbyte[] val2 = decoder.Int8Array();
        string val3 = decoder.String();
        sbyte[][] val4 = decoder.Array(decoder => decoder.Int8Array());

        Assert.Equal(16, val1);
        Assert.Equal([sbyte.MinValue, 0, sbyte.MaxValue], val2);
        Assert.Equal("Hello", val3);
        Assert.Equal([[12]], val4);
    }
}
