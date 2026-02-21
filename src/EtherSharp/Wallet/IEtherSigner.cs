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
    /// Attempts to sign the provided hash and write a 65-byte recoverable signature (<c>r</c> + <c>s</c> + <c>v</c>) to the destination buffer.
    /// </summary>
    /// <param name="data">The 32-byte hash to sign.</param>
    /// <param name="destination">The destination buffer for the recoverable signature.</param>
    /// <returns><see langword="true"/> when signing succeeds; otherwise, <see langword="false"/>.</returns>
    public bool TrySignRecoverable(ReadOnlySpan<byte> data, Span<byte> destination);
}
