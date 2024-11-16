using EtherSharp.Crypto.Curves;

namespace EtherSharp.Crypto;
public static class BIP32Curves
{
    public static readonly BIP32Curve ED25519 = new ED25519();
    public static readonly BIP32Curve Secp256k1 = new Secp256K1();
}