namespace EtherSharp.Types;

/// <summary>
/// Represents the result of searching a bounded range for an account nonce.
/// </summary>
public abstract record NonceSearchResult
{
    /// <summary>
    /// Indicates that the account nonce was found within the requested range.
    /// </summary>
    /// <param name="Nonce">The account nonce.</param>
    public sealed record Found(ulong Nonce) : NonceSearchResult;

    /// <summary>
    /// Indicates that the account nonce was not found within the requested range.
    /// </summary>
    public sealed record NotFound : NonceSearchResult;
}
