using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Precompiles;
using NSubstitute;

namespace EtherSharp.Tests.Interpreter.Precompiles;

public sealed class Ripemd160PrecompileTests
{
    [Theory]
    [InlineData("", "0000000000000000000000009c1185a5c5e9fc54612808977ee8f548b2258d31")]
    [InlineData("61", "0000000000000000000000000bdc9d2d256b3ee9daae347be6f4dc835a467ffe")]
    [InlineData("616263", "0000000000000000000000008eb208f7e05d987a9b044a8e98c6b087f15a0bfc")]
    [InlineData(
        "6d65737361676520646967657374",
        "0000000000000000000000005d0689ef49d2fae572b881b123a85ffa21595f36"
    )]
    [InlineData(
        "6162636465666768696a6b6c6d6e6f707172737475767778797a",
        "000000000000000000000000f71c27109c692c1b56bbdceb5b9d2865b3708dbc"
    )]
    [InlineData(
        "6162636462636465636465666465666765666768666768696768696a68696a6b696a6b6c6a6b6c6d6b6c6d6e6c6d6e6f6d6e6f706e6f7071",
        "00000000000000000000000012a053384a9c0c88e405a06c27dcf49ada62eb2b"
    )]
    [InlineData(
        "4142434445464748494a4b4c4d4e4f505152535455565758595a6162636465666768696a6b6c6d6e6f707172737475767778797a30313233343536373839",
        "000000000000000000000000b0e20b6e3116640286ed3a87a5713079b21f5189"
    )]
    [InlineData(
        "3132333435363738393031323334353637383930313233343536373839303132333435363738393031323334353637383930313233343536373839303132333435363738393031323334353637383930",
        "0000000000000000000000009b752e45573d4b39f4dbd3323cab82bf63326bfb"
    )]
    public async Task ExecuteAsync_ShouldMatchVector(string input, string expectedData)
    {
        var result = await Ripemd160Precompile.Instance.ExecuteAsync(
            Substitute.For<IInterpreterHost>(), default(PrecompileCall) with { Input = Convert.FromHexString(input), Gas = new GasBudget(UInt64.MaxValue) }
        );

        Assert.True(result.IsSuccess);
        Assert.Equal(Convert.FromHexString(expectedData), result.Data.ToArray());
    }
}
