namespace EtherSharp.Tx;

/// <summary>
/// Represents an error condition while trying to confirm a transaction on-chain.
/// </summary>
public record TxConfirmationError
{
    /// <summary>
    /// The transaction was not confirmed in a given timeout.
    /// </summary>
    public sealed record Timeout : TxConfirmationError;

    /// <summary>
    /// The nonce reserved for this transaction has already been used by another transaction.
    /// </summary>
    public sealed record NonceAlreadyUsed : TxConfirmationError;
}
