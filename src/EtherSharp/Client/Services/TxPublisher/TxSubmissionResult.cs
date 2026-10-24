using EtherSharp.Common.Exceptions;
using EtherSharp.Types;
using System.Buffers;

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
    private static readonly SearchValues<string> _alreadyExistsMessages = SearchValues.Create(
        [
            "ALREADY_EXISTS",
            "AlreadyKnown",
            "already known",
            "already imported",
            "tx already exists in cache",
            " known transaction",
            "existing txn with same hash",
            "duplicate tx"
        ],
        StringComparison.OrdinalIgnoreCase
    );

    private static readonly SearchValues<string> _transactionUnderpricedMessages = SearchValues.Create(
        [
            "underpriced",
            "fee too low",
            "gas price is too low",
            "gas price below configured minimum gas price",
            "gas price below current base fee",
            "max fee per gas less than block base fee",
            "could not replace existing tx"
        ],
        StringComparison.OrdinalIgnoreCase
    );

    private static readonly SearchValues<string> _nonceTooLowMessages = SearchValues.Create(
        ["nonce too low", "nonce is too low", "nonce has already been used", "next nonce", "invalid nonce"],
        StringComparison.OrdinalIgnoreCase
    );

    /// <summary>
    /// The transaction payload was accepted by the node and returned a transaction hash.
    /// </summary>
    /// <param name="TxHash">The hash returned by <c>eth_sendRawTransaction</c>.</param>
    public sealed record Success(Bytes32 TxHash) : TxSubmissionResult;

    /// <summary>
    /// The node reports the same transaction is already known.
    /// </summary>
    /// <remarks>
    /// This is treated as a successful publish state because the transaction is already in the node's mempool.
    /// </remarks>
    public sealed record AlreadyExists() : TxSubmissionResult;

    /// <summary>
    /// The node rejected the transaction because the effective gas price is too low.
    /// </summary>
    /// <remarks>
    /// Schedulers can respond by increasing fees and submitting a replacement transaction with the same nonce.
    /// </remarks>
    public sealed record TransactionUnderpriced() : TxSubmissionResult;

    /// <summary>
    /// The submitted nonce is lower than the account nonce known by the node.
    /// </summary>
    /// <remarks>
    /// This commonly means the nonce was already consumed by a mined or competing transaction.
    /// </remarks>
    public sealed record NonceTooLow() : TxSubmissionResult;

    /// <summary>
    /// The publish attempt failed for a reason that is not mapped to a specialized submission status.
    /// </summary>
    /// <param name="Exception">The original exception raised while publishing.</param>
    public sealed record UnhandledException(Exception Exception) : TxSubmissionResult;

    /// <summary>
    /// Converts an exception raised while submitting a transaction into a submission result.
    /// </summary>
    /// <param name="exception">The exception raised while submitting a transaction.</param>
    /// <returns>
    /// A recognized submission result for known <see cref="RPCException"/> messages, or
    /// <see cref="UnhandledException"/> containing the original exception otherwise.
    /// </returns>
    /// <remarks>
    /// Matching is case-insensitive and checks for already-known transactions first, then underpriced
    /// transactions, and finally nonce-too-low errors. Non-RPC exceptions are not classified by message.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/>.</exception>
    public static TxSubmissionResult FromException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if(exception is RPCException)
        {
            if(exception.Message.AsSpan().ContainsAny(_alreadyExistsMessages))
            {
                return new AlreadyExists();
            }
            else if(exception.Message.AsSpan().ContainsAny(_transactionUnderpricedMessages))
            {
                return new TransactionUnderpriced();
            }
            else if(exception.Message.AsSpan().ContainsAny(_nonceTooLowMessages))
            {
                return new NonceTooLow();
            }
        }

        return new UnhandledException(exception);
    }
}
