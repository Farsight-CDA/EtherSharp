using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.Encoder;
public class DynamicTupleEncoderTests
{
    private readonly AbiEncoder _encoder;

    public DynamicTupleEncoderTests()
    {
        _encoder = new AbiEncoder();
    }

    [Fact]
    public void Should_Match_StringTupleOutput()
    {
        byte[] expected = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000080000000000000000000000000000000000000000000000000000000000000000548656c6c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006576f726c64210000000000000000000000000000000000000000000000000000");

        byte[] actual = _encoder.DynamicTuple(x =>
        {
            x.String("Hello");
            x.String("World!");
        }).Build();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Throw_When_PassingOnlyFixedValues()
        => Assert.Throws<InvalidOperationException>(() => _encoder.DynamicTuple(x =>
        {
            x.Int16(3463);
            x.Int72(43666575);
        }).Build());

}
