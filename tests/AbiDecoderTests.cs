using EtherSharp.ABI.Decode;

namespace EtherSharp.Tests;
public class AbiDecoderTests
{

    [Fact]
    public void Test_int()
    {

        int bigIntValue = 123123;

        string @string = "0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000b68656c6c6f5f776f726c64000000000000000000000000000000000000000000";

        byte[] stringByte = Convert.FromHexString(@string);

        _ = new AbiDecoder(stringByte).Int8(out int actualOutput);


        Assert.Equal(bigIntValue, actualOutput);
    }
}

