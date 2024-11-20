﻿using EtherSharp.Crypto;
using Keysmith.Net.EC;
using Keysmith.Net.SLIP;
using Keysmith.Net.Wallet;

namespace EtherSharp.Wallet;
public class EtherHdWallet : BaseHdWallet, IEtherHdWallet
{
    private readonly byte[] _address;

    public string Address => Convert.ToBase64String(_address);

    ReadOnlySpan<byte> IEtherHdWallet.Address => throw new NotImplementedException();

    public EtherHdWallet(ReadOnlySpan<byte> privateKey)
        : base(Secp256k1.Instance, privateKey)
    {
        _address = new byte[20];
        InitAddress();
    }

    public EtherHdWallet(string mnemonic, uint accountIndex = 0, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase,
            Slip10.HardenedOffset + 44,
            Slip10.HardenedOffset + (uint) Slip44CoinType.Ethereum,
            Slip10.HardenedOffset,
            0,
            accountIndex)
    {
        _address = new byte[20];
        InitAddress();
    }

    public EtherHdWallet(string mnemonic, ReadOnlySpan<char> derivationPath, string passphrase = "")
    : base(Secp256k1.Instance, mnemonic, passphrase, derivationPath)
    {
        _address = new byte[20];
        InitAddress();
    }

    private void InitAddress()
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(_publicKey, hashBuffer);
        hashBuffer[^20..].CopyTo(_address);
    }
    public Span<byte> Sign(string data, out Span<byte> signature) => throw new NotImplementedException();
}
