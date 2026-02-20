using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.E2E;

public class ComplexEncodingTests
{
    [Fact]
    public void Should_RoundTrip_Multiple_Numbers()
    {
        byte[] encoded = new AbiEncoder()
            .UInt8(Byte.MaxValue)
            .UInt16(Byte.MaxValue)
            .UInt24(50)
            .UInt32(Byte.MinValue)
            .UInt40(Byte.MinValue)
            .Build();

        var decoder = new AbiDecoder(encoded);
        byte a = decoder.UInt8();
        ushort b = decoder.UInt16();
        uint c = decoder.UInt24();
        uint d = decoder.UInt32();
        ulong e = decoder.UInt40();

        Assert.Equal(Byte.MaxValue, a);
        Assert.Equal(Byte.MaxValue, b);
        Assert.Equal((uint) 50, c);
        Assert.Equal(Byte.MinValue, d);
        Assert.Equal(Byte.MinValue, e);
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
            .Int8Array(SByte.MinValue, 0, SByte.MaxValue)
            .String("Hello")
            .Array<byte[]>([[12]], (encoder, value) => encoder.Int8Array(12))
            .Build();

        var decoder = new AbiDecoder(encoded);

        int val1 = decoder.Int32();
        sbyte[] val2 = decoder.Int8Array();
        string val3 = decoder.String();
        sbyte[][] val4 = decoder.Array(decoder => decoder.Int8Array());

        Assert.Equal(16, val1);
        Assert.Equal([SByte.MinValue, 0, SByte.MaxValue], val2);
        Assert.Equal("Hello", val3);
        Assert.Equal([[12]], val4);
    }

    [Fact]
    public void Should_RoundTrip_DynamicTuple_Array()
    {
        string[] input = ["A", "B", "C"];
        byte[] encoded = new AbiEncoder()
            .Array(
                input,
                (encoder, value) => encoder.DynamicTuple(
                    encoder => encoder.String(value)))
            .Build();

        var decoder = new AbiDecoder(encoded);
        string[] output = decoder.Array(decoder => decoder.DynamicTuple(x => x.String()));

        Assert.Equal(input, output);
    }
}
