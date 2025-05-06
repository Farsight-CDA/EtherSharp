using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.Encoder;
public class ArrayAbiEncoderTests
{
    private readonly AbiEncoder _encoder;

    public ArrayAbiEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Should_Match_Empty_Array()
    {
        byte[] expected = Convert.FromHexString("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000000");
        byte[] actual = _encoder
            .Array<object>([], (_, _) => { })
            .Build();

        Assert.Equal(expected, actual);
    }
}
