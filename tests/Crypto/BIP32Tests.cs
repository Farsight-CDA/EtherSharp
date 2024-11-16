using EtherSharp.Crypto;

namespace EtherSharp.Tests.Crypto;
public class BIP32Tests
{
    [Fact]
    public void RunBIP32Tests()
    {
        var (key, _) = BIP32Curves.Secp256k1.DerivePath(
            BIP39.MnemonicToSeed("ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal"),
            $"m/44'/60'/0'/0/0"
        );

        Assert.Equal("ab4accc9310d90a61fc354d8f353bca4a2b3c0590685d3eb82d0216af3badddc", Convert.ToHexString(key), ignoreCase: true);
    }
}
