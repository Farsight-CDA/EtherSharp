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
    /// Attempts to sign the provided hash and write a 64-byte signature (<c>r</c> + <c>s</c>) to the destination buffer.
    /// </summary>
    /// <param name="data">The 32-byte hash to sign.</param>
    /// <param name="destination">The destination buffer for the signature.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySign(ReadOnlySpan<byte> data, Span<byte> destination);

    /// <summary>
    /// Attempts to sign the provided hash and write a 64-byte signature (<c>r</c> + <c>s</c>) to the destination buffer.
    /// </summary>
    /// <param name="data">The hash to sign.</param>
    /// <param name="destination">The destination buffer for the signature.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySign(in Bytes32 data, Span<byte> destination)
        => TrySign(data.DangerousGetReadOnlySpan(), destination);

    /// <summary>
    /// Attempts to sign the provided hash and write a canonical low-<c>s</c>, 65-byte recoverable signature (<c>r</c> + <c>s</c> + <c>v</c>) to the destination buffer.
    /// </summary>
    /// <param name="data">The 32-byte hash to sign.</param>
    /// <param name="destination">The destination buffer for the recoverable signature.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySignRecoverable(ReadOnlySpan<byte> data, Span<byte> destination);

    /// <summary>
    /// Attempts to sign the provided hash and write a canonical low-<c>s</c>, 65-byte recoverable signature (<c>r</c> + <c>s</c> + <c>v</c>) to the destination buffer.
    /// </summary>
    /// <param name="data">The hash to sign.</param>
    /// <param name="destination">The destination buffer for the recoverable signature.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySignRecoverable(in Bytes32 data, Span<byte> destination)
        => TrySignRecoverable(data.DangerousGetReadOnlySpan(), destination);

    /// <summary>
    /// Attempts to sign an EIP-712 message with a recoverable signature.
    /// </summary>
    /// <typeparam name="TMessage">Source-generated EIP-712 message type.</typeparam>
    /// <param name="domain">Signature domain.</param>
    /// <param name="message">Typed message to hash and sign.</param>
    /// <param name="destination">The destination buffer for the 65-byte recoverable signature, with <c>v</c> normalized to 27 or 28.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySignEIP712<TMessage>(
        in EIP712Domain domain,
        in TMessage message,
        Span<byte> destination)
        where TMessage : IEIP712Type
    {
        if(destination.Length != 65)
        {
            return false;
        }

        var hash = message.GetSigningHash(domain);
        if(!TrySignRecoverable(hash, destination))
        {
            return false;
        }

        if(destination[64] <= 1)
        {
            destination[64] += 27;
        }

        return destination[64] is 27 or 28;
    }
}
