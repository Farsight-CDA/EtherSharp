using EtherSharp.Crypto;
using EtherSharp.Types;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    /// Signs an EIP-191 personal message (version 0x45) with a recoverable signature.
    /// </summary>
    /// <param name="message">Raw message bytes; the byte count is included in the signed digest.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The signature, with its recovery identifier normalized to 27 or 28.</returns>
    public ValueTask<RecoverableEtherSignature> SignPersonalMessageAsync(
        ReadOnlySpan<byte> message,
        CancellationToken cancellationToken = default
    ) => NormalizeRecoveryIdAsync(
        SignRecoverableAsync(
            EIP191.HashPersonalMessage(message),
            cancellationToken
        )
    );

    /// <summary>
    /// Signs a UTF-8 EIP-191 personal message (version 0x45) with a recoverable signature.
    /// </summary>
    /// <param name="message">Text to sign as UTF-8.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The signature, with its recovery identifier normalized to 27 or 28.</returns>
    public ValueTask<RecoverableEtherSignature> SignPersonalMessageAsync(
        string message,
        CancellationToken cancellationToken = default
    ) => NormalizeRecoveryIdAsync(
        SignRecoverableAsync(
            EIP191.HashPersonalMessage(message),
            cancellationToken
        )
    );

    /// <summary>
    /// Signs EIP-191 data bound to an intended validator (version 0x00).
    /// </summary>
    /// <param name="validator">The intended validator address.</param>
    /// <param name="data">Raw data to sign.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The signature, with its recovery identifier normalized to 27 or 28.</returns>
    public ValueTask<RecoverableEtherSignature> SignIntendedValidatorAsync(
        Address validator,
        ReadOnlySpan<byte> data,
        CancellationToken cancellationToken = default
    ) => NormalizeRecoveryIdAsync(
        SignRecoverableAsync(
            EIP191.HashIntendedValidator(validator, data),
            cancellationToken
        )
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
        => NormalizeRecoveryIdAsync(
            SignRecoverableAsync(
                message.GetSigningHash(domain),
                cancellationToken
            )
        );

    private static async ValueTask<RecoverableEtherSignature> NormalizeRecoveryIdAsync(
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
