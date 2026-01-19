using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.Decoder;

public class BackwardOffsetTests
{
    [Fact]
    public void Should_Support_Backward_Offsets()
    {
        byte[] payload = new byte[64];
        payload[31] = 0x20;         
        var decoder = new AbiDecoder(payload);

        var num = decoder.Number<uint>(true, 32);
        Assert.Equal(32u, num);
        
        var bytes = decoder.Bytes().ToArray();
        
        Assert.Equal(32, bytes.Length);
        Assert.All(bytes, b => Assert.Equal(0, b));
    }
}
