using EtherSharp.ABI;
using EtherSharp.ABI.Decode;

namespace EtherSharp.Tests.ABI.E2E;
public class ComplexEncodingTests
{

    [Fact]
    public void Multible_Numbers()
    {
        var abiEncoder = new AbiEncoder()
            .UInt8(byte.MaxValue)
            .UInt8(byte.MaxValue)
            .UInt8(50)
            .UInt8(byte.MinValue)
            .UInt8(byte.MinValue);
        ;
        byte[] buffer = abiEncoder.Build();

        _ = new AbiDecoder(buffer)
            .UInt8(out byte a)
            .UInt8(out byte b)
            .UInt8(out byte c)
            .UInt8(out byte d)
            .UInt8(out byte e);

        Assert.Equal(byte.MaxValue, a);
        Assert.Equal(byte.MaxValue, b);
        Assert.Equal(50, c);
        Assert.Equal(byte.MinValue, d);
        Assert.Equal(byte.MinValue, e);
    }
}
