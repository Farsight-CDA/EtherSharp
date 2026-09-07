using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Precompiles;
using NSubstitute;

namespace EtherSharp.Tests.Interpreter.Precompiles;

public sealed class Blake2FPrecompileTests
{
    private const string STATE_MESSAGE_COUNTERS =
        "48c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5"
        + "d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b"
        + "6162630000000000000000000000000000000000000000000000000000000000"
        + "0000000000000000000000000000000000000000000000000000000000000000"
        + "0000000000000000000000000000000000000000000000000000000000000000"
        + "0000000000000000000000000000000000000000000000000000000000000000"
        + "03000000000000000000000000000000";

    // https://github.com/ethereum/go-ethereum/blob/master/core/vm/testdata/precompiles/blake2F.json
    [Theory]
    [InlineData("vector 4", "00000000" + STATE_MESSAGE_COUNTERS + "01",
        "08c9bcf367e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5"
        + "d282e6ad7f520e511f6c3e2b8c68059b9442be0454267ce079217e1319cde05b"
    )]
    [InlineData("vector 5", "0000000c" + STATE_MESSAGE_COUNTERS + "01",
        "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d1"
        + "7d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923"
    )]
    [InlineData("vector 6", "0000000c" + STATE_MESSAGE_COUNTERS + "00",
        "75ab69d3190a562c51aef8d88f1c2775876944407270c42c9844252c26d28752"
        + "98743e7f6d5ea2f2d3e8d226039cd31b4e426ac4f2d3d666a610c2116fde4735"
    )]
    [InlineData("vector 7", "00000001" + STATE_MESSAGE_COUNTERS + "01",
        "b63a380cb2897d521994a85234ee2c181b5f844d2c624c002677e9703449d2fb"
        + "a551b3a8333bcdf5f2f7e08993d53923de3d64fcc68c034e717b9293fed7a421"
    )]
    [InlineData("vector 8", "007A1200" + STATE_MESSAGE_COUNTERS + "01",
        "6d2ce9e534d50e18ff866ae92d70cceba79bbcd14c63819fe48752c8aca87a4b"
        + "b7dcc230d22a4047f0486cfcfb50a17b24b2899eb8fca370f22240adb5170189"
    )]
    public async Task ExecuteAsync_ShouldMatchOfficialVector(string name, string input, string expected)
    {
        var result = await Blake2FPrecompile.Instance.ExecuteAsync(
            Substitute.For<IInterpreterHost>(), default(PrecompileCall) with { Input = Convert.FromHexString(input) }
        );

        Assert.True(result.Success, name);
        Assert.Equal(Convert.FromHexString(expected), result.Data.ToArray());
    }
}
