using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Precompiles;
using NSubstitute;
using System.Text.Json;

namespace EtherSharp.Tests.Interpreter.Precompiles;

public sealed class P256VerifyPrecompileTests
{
    public static TheoryData<string, string, string> Vectors
    {
        get {
            using var stream = typeof(P256VerifyPrecompileTests).Assembly.GetManifestResourceStream(
                "EtherSharp.Tests.Interpreter.Precompiles.Vectors.p256verify.json"
            )!;
            using var document = JsonDocument.Parse(stream);
            var vectors = new TheoryData<string, string, string>();
            foreach(var vector in document.RootElement.EnumerateArray())
            {
                vectors.Add(
                    vector.GetProperty("Name").GetString()!,
                    vector.GetProperty("Input").GetString()!,
                    vector.GetProperty("Expected").GetString()!
                );
            }
            return vectors;
        }
    }

    [Theory]
    [MemberData(nameof(Vectors))]
    public async Task ExecuteAsync_ShouldMatchOfficialVector(string name, string input, string expected)
    {
        var result = await P256VerifyPrecompile.Instance.ExecuteAsync(
            Substitute.For<IInterpreterHost>(), default(PrecompileCall) with { Input = Convert.FromHexString(input) }
        );

        Assert.True(result.Success, name);
        Assert.Equal(Convert.FromHexString(expected), result.Data.ToArray());
    }
}
