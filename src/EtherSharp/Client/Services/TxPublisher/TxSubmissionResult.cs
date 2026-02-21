namespace EtherSharp.Client.Services.TxPublisher;

/// <summary>
/// Represents the outcome of broadcasting a signed transaction payload.
/// </summary>
/// <remarks>
/// The scheduler uses this result to choose its next action: continue waiting for confirmation,
/// reprice and retry, or resolve completion through receipt polling.
/// </remarks>
public abstract record TxSubmissionResult
{
    /// <summary>
    /// The transaction payload was accepted by the node and returned a transaction hash.
    /// </summary>
    /// <param name="TxHash">The hash returned by <c>eth_sendRawTransaction</c>.</param>
    public record Success(string TxHash) : TxSubmissionResult;

    /// <summary>
    /// The node reports the same transaction is already known.
    /// </summary>
    /// <remarks>
    /// This is treated as a successful publish state because the transaction is already in the node's mempool.
    /// </remarks>
    public record AlreadyExists() : TxSubmissionResult;

    /// <summary>
    /// The node rejected the transaction because the effective gas price is too low.
    /// </summary>
    /// <remarks>
    /// Schedulers can respond by increasing fees and submitting a replacement transaction with the same nonce.
    /// </remarks>
    public record TransactionUnderpriced() : TxSubmissionResult;

    /// <summary>
    /// The submitted nonce is lower than the account nonce known by the node.
    /// </summary>
    /// <remarks>
    /// This commonly means the nonce was already consumed by a mined or competing transaction.
    /// </remarks>
    public record NonceTooLow() : TxSubmissionResult;

    /// <summary>
    /// The publish attempt failed for a reason that is not mapped to a specialized submission status.
    /// </summary>
    /// <param name="Exception">The original exception raised while publishing.</param>
    public record UnhandledException(Exception Exception) : TxSubmissionResult;
}
