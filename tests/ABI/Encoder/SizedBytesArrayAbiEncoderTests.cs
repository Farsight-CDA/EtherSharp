using EtherSharp.ABI;
using EtherSharp.Types;

namespace EtherSharp.Tests.ABI.Encoder;

public class SizedBytesArrayAbiEncoderTests
{
    [Fact]
    public void Should_Encode_Bytes1_Array_With_Multiple_Elements()
    {
        byte[] expected = Convert.FromHexString(
            "0000000000000000000000000000000000000000000000000000000000000020" +
            "0000000000000000000000000000000000000000000000000000000000000002" +
            "1200000000000000000000000000000000000000000000000000000000000000" +
            "3400000000000000000000000000000000000000000000000000000000000000");

        byte[] actual = new AbiEncoder().Bytes1Array(Bytes1.FromBytes([0x12]), Bytes1.FromBytes([0x34])).Build();

        Assert.Equal(expected, actual);
    }
}
