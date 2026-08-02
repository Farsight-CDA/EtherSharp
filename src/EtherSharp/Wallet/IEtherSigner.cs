using EtherSharp.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Wallet;

/// <summary>
/// Defines the contract for an Ethereum signer that can produce standard and recoverable signatures.
/// </summary>
public interface IEtherSigner
{
    /// <summary>
    /// Gets the wallet address associated with this signer.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Signs the provided hash.
    /// </summary>
    /// <param name="hash">The 32-byte hash to sign.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The signature.</returns>
    public ValueTask<EtherSignature> SignAsync(
        Bytes32 hash,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Signs the provided hash with a canonical low-<c>s</c>, recoverable signature.
    /// </summary>
    /// <param name="hash">The 32-byte hash to sign.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The recoverable signature.</returns>
    public ValueTask<RecoverableEtherSignature> SignRecoverableAsync(
        Bytes32 hash,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Signs an EIP-712 message with a recoverable signature.
    /// </summary>
    /// <typeparam name="TMessage">Source-generated EIP-712 message type.</typeparam>
    /// <param name="domain">Signature domain.</param>
    /// <param name="message">Typed message to hash and sign.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The recoverable signature, with its recovery identifier normalized to 27 or 28.</returns>
    public ValueTask<RecoverableEtherSignature> SignEIP712Async<TMessage>(
        in EIP712Domain domain,
        in TMessage message,
        CancellationToken cancellationToken = default
    ) where TMessage : IEIP712Type
    {
        var hash = message.GetSigningHash(domain);
        return NormalizeEIP712SignatureAsync(SignRecoverableAsync(hash, cancellationToken));
    }

    private static async ValueTask<RecoverableEtherSignature> NormalizeEIP712SignatureAsync(
        ValueTask<RecoverableEtherSignature> signatureTask)
    {
        var signature = await signatureTask.ConfigureAwait(false);
        return signature.RecoveryId switch
        {
            0 or 1 => signature with { RecoveryId = (byte) (signature.RecoveryId + 27) },
            27 or 28 => signature,
            _ => throw new NotSupportedException("Bad recovery identifier")
        };
    }
}
