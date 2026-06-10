using EtherSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for a Solidity ABI error type.
/// </summary>
/// <typeparam name="TSelf">The Solidity error type.</typeparam>
public interface ISolidityError<TSelf>
    where TSelf : ISolidityError<TSelf>
{
    /// <summary>
    /// Error signature used to calculate the signature bytes.
    /// </summary>
    public abstract static string ErrorSignature { get; }

    /// <summary>
    /// Hex encoded error selector based on the error signature.
    /// </summary>
    public abstract static string SelectorHex { get; }

    /// <summary>
    /// Gets the error selector bytes.
    /// </summary>
    public abstract static Bytes4 Selector { get; }

    /// <summary>
    /// Decodes error data into the Solidity error type.
    /// </summary>
    /// <param name="data">Error data including selector and ABI-encoded arguments.</param>
    /// <returns>The decoded error.</returns>
    public abstract static TSelf Decode(ReadOnlyMemory<byte> data);

    /// <summary>
    /// Encodes this error as Solidity revert data.
    /// </summary>
    /// <returns>Error data including selector and ABI-encoded arguments.</returns>
    public byte[] Encode();

    /// <summary>
    /// Checks whether the error data starts with this error's selector bytes.
    /// </summary>
    /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
    /// <returns><see langword="true" /> when the selector matches; otherwise <see langword="false" />.</returns>
    public abstract static bool IsMatchingSelector(ReadOnlySpan<byte> errorData);

    /// <summary>
    /// Attempts to decode error data into the Solidity error type.
    /// </summary>
    /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
    /// <param name="parsedError">The decoded error when successful.</param>
    /// <returns><see langword="true" /> when the selector matches and the error was decoded; otherwise <see langword="false" />.</returns>
    public abstract static bool TryDecode(ReadOnlyMemory<byte> errorData, [MaybeNullWhen(false)] out TSelf parsedError);
}
