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
    /// There already is a transaction with this nonce in the mempool and the provided gas fee is not high enough to replace it.
    /// </summary>
    public sealed record ReplacementTransactionUnderpriced : TxConfirmationError;

    /// <summary>
    /// Represents any error condition not represented by the other TxConfirmationError types.
    /// </summary>
    public sealed record UnhandledException(Exception Exception) : TxConfirmationError;
}
