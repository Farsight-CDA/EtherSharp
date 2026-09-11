using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Precompiles;
using NSubstitute;

namespace EtherSharp.Tests.Interpreter.Precompiles;

public sealed class ModExpPrecompileTests
{
    [Theory]
    [InlineData("", true, "")]
    [InlineData("01", false, "")] // Right-padding this partial header makes the gas cost unaffordable.
    [InlineData( // 2^5 mod 13 = 6.
        "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "02050d",
        true, "06"
    )]
    [InlineData( // Zero modulus returns zero.
        "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "020500",
        true, "00"
    )]
    [InlineData( // 0^0 mod 13 = 1, with empty base and exponent.
        "0000000000000000000000000000000000000000000000000000000000000000"
        + "0000000000000000000000000000000000000000000000000000000000000000"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0d",
        true, "01"
    )]
    [InlineData( // Truncated modulus is right-padded to 0x0100: 2^3 mod 256 = 8.
        "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000002"
        + "020301",
        true, "0008"
    )]
    [InlineData( // Leading zeros permit UInt256 arithmetic while preserving the 33-byte output.
        "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000021"
        + "020500"
        + "000000000000000000000000000000000000000000000000000000000000000d",
        true, "00" + "0000000000000000000000000000000000000000000000000000000000000006"
    )]
    [InlineData( // Wide operands: (2^256)^2 mod (2^256 + 1) = 1.
        "0000000000000000000000000000000000000000000000000000000000000021"
        + "0000000000000000000000000000000000000000000000000000000000000001"
        + "0000000000000000000000000000000000000000000000000000000000000021"
        + "01" + "0000000000000000000000000000000000000000000000000000000000000000"
        + "02"
        + "01" + "0000000000000000000000000000000000000000000000000000000000000001",
        true, "00" + "0000000000000000000000000000000000000000000000000000000000000001"
    )]
    public async Task ExecuteAsync_ShouldMatchVector(string input, bool expectedSuccess, string expectedData)
    {
        var result = await new ModExpPrecompile(maxOperandLength: 1024).ExecuteAsync(
            Substitute.For<IInterpreterHost>(), default(PrecompileCall) with { Input = Convert.FromHexString(input), Gas = new GasBudget(UInt64.MaxValue) }
        );

        Assert.Equal(expectedSuccess, result.IsSuccess);
        Assert.Equal(Convert.FromHexString(expectedData), result.Data.ToArray());
        if(!expectedSuccess)
        {
            Assert.True(result.IsExceptionalHalt(out var reason));
            Assert.Equal(ExceptionalHaltReason.OutOfGas, reason);
        }
    }
}
