using EtherSharp.Crypto;
using EtherSharp.Types;
using Keysmith.Net.EC;
using Keysmith.Net.SLIP;
using Keysmith.Net.Wallet;
using System.Security.Cryptography;

namespace EtherSharp.Wallet;

/// <summary>
/// Represents an HD Ethereum wallet backed by a secp256k1 private key.
/// </summary>
public sealed class EtherHdWallet : BaseWeierstrassHdWallet<Secp256k1>, IEtherSigner
{
    /// <summary>
    /// Gets the Ethereum address derived from the current wallet public key.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Creates a new wallet using a cryptographically random private key.
    /// </summary>
    /// <returns>A new <see cref="EtherHdWallet"/> instance.</returns>
    public static EtherHdWallet Create()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return new EtherHdWallet(key);
    }

    /// <summary>
    /// Initializes a wallet from a raw private key.
    /// </summary>
    /// <param name="privateKey">The 32-byte secp256k1 private key.</param>
    public EtherHdWallet(ReadOnlySpan<byte> privateKey)
        : base(Secp256k1.Instance, privateKey)
    {
        Address = GenerateAddress();
    }

    /// <summary>
    /// Initializes a wallet from a mnemonic using the default Ethereum account derivation path.
    /// </summary>
    /// <param name="mnemonic">The BIP-39 mnemonic phrase.</param>
    /// <param name="accountIndex">The account index used in the derivation path.</param>
    /// <param name="passphrase">The optional mnemonic passphrase.</param>
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

    /// <summary>
    /// Initializes a wallet from a mnemonic using a custom derivation path.
    /// </summary>
    /// <param name="mnemonic">The BIP-39 mnemonic phrase.</param>
    /// <param name="derivationPath">The derivation path to use for key derivation.</param>
    /// <param name="passphrase">The optional mnemonic passphrase.</param>
    public EtherHdWallet(string mnemonic, ReadOnlySpan<char> derivationPath, string passphrase = "")
        : base(Secp256k1.Instance, mnemonic, passphrase, derivationPath)
    {
        Address = GenerateAddress();
    }

    private Address GenerateAddress()
    {
        var pkSpan = _uncompressedPublicKey.AsSpan();

        pkSpan[..32].Reverse();
        pkSpan[32..64].Reverse();

        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(_uncompressedPublicKey, hashBuffer);
        return Address.FromBytes(hashBuffer[^20..]);
    }

    ValueTask<EtherSignature> IEtherSigner.SignAsync(Bytes32 hash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Span<byte> signatureBuffer = stackalloc byte[64];
        if(!TrySign(hash.DangerousGetReadOnlySpan(), signatureBuffer))
        {
            throw new CryptographicException("Failed to sign hash.");
        }

        signatureBuffer[..32].Reverse();
        signatureBuffer[32..64].Reverse();

        var signature = new EtherSignature(
            Bytes32.FromBytes(signatureBuffer[..32]),
            Bytes32.FromBytes(signatureBuffer[32..])
        );
        return ValueTask.FromResult(signature);
    }

    ValueTask<RecoverableEtherSignature> IEtherSigner.SignRecoverableAsync(
        Bytes32 hash,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Span<byte> signatureBuffer = stackalloc byte[65];
        if(!TrySignRecoverable(hash.DangerousGetReadOnlySpan(), signatureBuffer))
        {
            throw new CryptographicException("Failed to sign hash.");
        }

        signatureBuffer[..32].Reverse();
        signatureBuffer[32..64].Reverse();

        var signature = new RecoverableEtherSignature(
            Bytes32.FromBytes(signatureBuffer[..32]),
            Bytes32.FromBytes(signatureBuffer[32..64]),
            signatureBuffer[64]
        );
        return ValueTask.FromResult(signature);
    }
}
