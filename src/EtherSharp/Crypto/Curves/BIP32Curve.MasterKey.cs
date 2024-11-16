using System.Security.Cryptography;
using System.Text;

namespace EtherSharp.Crypto.Curves;
public abstract partial class BIP32Curve
{
    public (byte[] Key, byte[] ChainCode) DeriveMasterKey(ReadOnlySpan<byte> seed)
    {
        byte[] key = new byte[32];
        byte[] chainCode = new byte[32];
        GetMasterKeyFromSeed(seed, key, chainCode);

        return (key, chainCode);
    }

    //https://github.com/bitcoin/bips/blob/master/bip-0032.mediawiki#master-key-generation
    private void GetMasterKeyFromSeed(ReadOnlySpan<byte> seed, Span<byte> keyDestination, Span<byte> chainCodeDestination)
    {
        Span<byte> curveBuffer = stackalloc byte[Encoding.ASCII.GetByteCount(Name)];
        Span<byte> buffer = stackalloc byte[64];

        Encoding.ASCII.GetBytes(Name, curveBuffer);
        _ = HMACSHA512.HashData(curveBuffer, seed, buffer);

        var il = buffer[..32];
        var ir = buffer[32..];

        if(!IsValidKey(il))
        {
            GetMasterKeyFromSeed(buffer, il, ir);
        }

        il.CopyTo(keyDestination);
        ir.CopyTo(chainCodeDestination);
    }
}
