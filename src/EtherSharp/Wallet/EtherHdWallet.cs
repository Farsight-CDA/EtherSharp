using EtherSharp.Crypto;
using Keysmith.Net.EC;
using Keysmith.Net.SLIP;
using Keysmith.Net.Wallet;

namespace EtherSharp.Wallet;
public class EtherHdWallet : BaseHdWallet, IEtherSigner
{
    private readonly byte[] _addressBytes = null!;
    public string Address { get; } = null!;

    public EtherHdWallet(ReadOnlySpan<byte> privateKey)
        : base(Secp256k1.Instance, privateKey)
    {
        _addressBytes = new byte[20];
        InitAddress();
        Address = $"0x{Convert.ToHexString(_addressBytes)}";
    }

    public EtherHdWallet(string mnemonic, uint accountIndex = 0, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase,
            Slip10.HardenedOffset + 44,
            Slip10.HardenedOffset + (uint) Slip44CoinType.Ethereum,
            Slip10.HardenedOffset,
            0,
            accountIndex)
    {
        _addressBytes = new byte[20];
        InitAddress();
        Address = $"0x{Convert.ToHexString(_addressBytes)}";
    }

    public EtherHdWallet(string mnemonic, ReadOnlySpan<char> derivationPath, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase, derivationPath)
    {
        _addressBytes = new byte[20];
        InitAddress();
        Address = $"0x{Convert.ToHexString(_addressBytes)}";
    }

    private void InitAddress()
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(_publicKey, hashBuffer);
        hashBuffer[^20..].CopyTo(_addressBytes);

    }
}
