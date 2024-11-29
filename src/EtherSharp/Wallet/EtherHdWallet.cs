﻿using EtherSharp.Crypto;
using EtherSharp.Types;
using Keysmith.Net.EC;
using Keysmith.Net.SLIP;
using Keysmith.Net.Wallet;
using System.Security.Cryptography;

namespace EtherSharp.Wallet;
public class EtherHdWallet : BaseWeierstrassHdWallet<Secp256k1>, IEtherSigner
{
    public Address Address { get; } = null!;

    public static EtherHdWallet Create()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return new EtherHdWallet(key);
    }

    public EtherHdWallet(ReadOnlySpan<byte> privateKey)
        : base(Secp256k1.Instance, privateKey)
    {
        Address = GenerateAddress();
    }

    public EtherHdWallet(string mnemonic, uint accountIndex = 0, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase,
            Slip10.HardenedOffset + 44,
            Slip10.HardenedOffset + (uint) Slip44CoinType.Ethereum,
            Slip10.HardenedOffset,
            0,
            accountIndex)
    {
        Address = GenerateAddress();
    }

    public EtherHdWallet(string mnemonic, ReadOnlySpan<char> derivationPath, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase, derivationPath)
    {
        Address = GenerateAddress();
    }

    private Address GenerateAddress()
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(_uncompressedPublicKey, hashBuffer);
        return Address.FromBytes(hashBuffer[^20..]);
    }
}
