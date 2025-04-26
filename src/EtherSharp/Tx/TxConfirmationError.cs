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
    /// The transaction fee is too low to be included in the mempool.
    /// </summary>
    public sealed record TransactionUnderpriced : TxConfirmationError;

    /// <summary>
    /// Represents any error condition not represented by the other TxConfirmationError types.
    /// </summary>
    public sealed record UnhandledException(Exception Exception) : TxConfirmationError;
}
