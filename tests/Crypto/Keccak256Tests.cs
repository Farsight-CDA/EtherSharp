using EtherSharp.Crypto;
using System.Text;

namespace EtherSharp.Tests.Crypto;
public class Keccak256Tests
{
    [Theory]
    [InlineData("", "c5d2460186f7233c927e7db2dcc703c0e500b653ca82273b7bfad8045d85a470")]
    [InlineData("123", "64e604787cbf194841e7b68d7cd28786f6c9a0a3ab9f8b0a0e87cb4387ab0107")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "e24c801be1f60170f6130e4cd29d2a51a1898724c5612e382d1e9be08f7cd6b7")]
    public void Should_Match_Keccak256_Output(string input, string expected)
    {
        string actual = Convert.ToHexString(
            Keccak256.HashData(Encoding.UTF8.GetBytes(input))
        );

        Assert.Equal(expected, actual, ignoreCase: true);
    }
}
