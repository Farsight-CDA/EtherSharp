using EtherSharp.ABI;
using EtherSharp.ABI.Decode;

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
        _ = new AbiDecoder(encoded)
            .UInt8(out byte a)
            .UInt16(out ushort b)
            .UInt24(out uint c)
            .UInt32(out uint d)
            .UInt40(out ulong e);

        Assert.Equal(byte.MaxValue, a);
        Assert.Equal(byte.MaxValue, b);
        Assert.Equal((uint) 50, c);
        Assert.Equal(byte.MinValue, d);
        Assert.Equal(byte.MinValue, e);
    }

    [Fact]
    public void Should_RoundTrip_Nested_Int32_Array()
    {
        int[] input = [1, 2, 3, 4, 5];
        byte[] encoded = new AbiEncoder()
            .Array(x => 
                x.Array(y => 
                    y.Int32Array(input)))
            .Build();
        _ = new AbiDecoder(encoded)
            .Array(out int[] output, x =>
                x.Array(y =>
                    y.Int32Array()));

        Assert.Equal(input, output);
    }
}
