using EtherSharp.Types;

namespace EtherSharp.Tx;

/// <summary>
/// Final outcome of <c>PublishAndConfirmAsync</c> for a pending transaction nonce.
/// </summary>
/// <remarks>
/// A successful confirmation returns a receipt; otherwise the result explains why the nonce advanced
/// without a matching receipt, was cancelled before publish, or failed with an unexpected exception.
/// </remarks>
public record TxConfirmationResult
{
    /// <summary>
    /// The transaction was confirmed and a receipt was found.
    /// </summary>
    /// <param name="Receipt">The confirmed transaction receipt.</param>
    public record Success(TxReceipt Receipt) : TxConfirmationResult;

    /// <summary>
    /// The nonce was consumed, but no receipt was found for tracked submissions.
    /// </summary>
    public record NonceAlreadyUsed() : TxConfirmationResult;

    /// <summary>
    /// The transaction was cancelled before it became non-cancellable.
    /// </summary>
    public record Cancelled() : TxConfirmationResult;

    /// <summary>
    /// An unexpected exception interrupted confirmation.
    /// </summary>
    /// <param name="Exception">The original exception.</param>
    public record UnhandledException(Exception Exception) : TxConfirmationResult;

    /// <summary>
    /// Gets the receipt for a successful confirmation result.
    /// </summary>
    /// <returns>The confirmed transaction receipt.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this result is not <see cref="Success"/>.</exception>
    public TxReceipt GetReceipt()
        => this is Success successResult
            ? successResult.Receipt
            : throw new InvalidOperationException($"{nameof(TxConfirmationResult)} is of type {GetType().Name}");
}
