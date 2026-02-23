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

    [Fact]
    public void Should_Match_Keccak256_Output_For_Long_Zeroed_Inputs()
    {
        string[] expected =
        [
            "54a8c0ab653c15bfb48b47fd011ba2b9617af01cb45cab344acd57c924d56798",
            "e8e77626586f73b955364c7b4bbf0bb7f7685ebd40e852b164633a4acbd3244c",
            "011b4d03dd8c01f1049143cf9c4c817e4b167f1d1b83e5c6f0f10d89ba1e7bce",
            "f490de2920c8a35fabeb13208852aa28c76f9be9b03a4dd2b3c075f7a26923b4",
            "290decd9548b62a8d60345a988386fc84ba6bc95484008f6362f93160ef3e563",
            "ad3228b676f7d3cd4284a5443f17f1962b36e491b30a40b2405849e597ba5fb5",
            "012893657d8eb2efad4de0a91bcd0e39ad9837745dec3ea923737ea803fc8e3d",
            "d397b3b043d87fcd6fad1291ff0bfd16401c274896d8c63a923727f077b8e0b5",
            "d5c44f659751a819616c58c9efe38e80f2b84cf621036da99c019bbe4f1fb647",
            "b5d4d1df10388bbc208778ff02310db98fdaa68efed0b2068a9bef78bd3bfd74",
            "aeffb38c06e111d84216396baefeb7fed397f303d5cb84a33f1e8b485c4a22da",
            "a8bae11751799de4dbe638406c5c9642c0e791f2a65e852a05ba4fdf0d88e3e6",
            "18c0afa1b8c5fd0b8963bfa3bf15b4e2c18ded2e129bca2783ddd2d9921832b3",
            "291ec7ae1d17299b418e889d0e5c003ebad587ecbedf6f3c7f8b898b52318f06",
            "8cd8aa91369996e33a91695c8fcec3c5597c2fd76edc9f9d4d604fe2624cf914",
            "66114d98de5a9683f3dc3c5859edfc4b35c1ea16d6afef00b74ae6ec812c2594"
        ];

        for(int i = 1; i <= 16; i++)
        {
            byte[] input = new byte[1 << i];

            string actual = Convert.ToHexString(
                Keccak256.HashData(input)
            );

            Assert.Equal(expected[i - 1], actual, ignoreCase: true);
        }
    }
}
