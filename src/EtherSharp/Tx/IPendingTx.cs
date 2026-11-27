using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using System.Collections.Immutable;

namespace EtherSharp.Tx;

/// <summary>
/// Represents a prepared or in-flight logical transaction with single-step publish, reprice, and confirmation operations.
/// </summary>
/// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
/// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
public interface IPendingTx<TTxParams, TTxGasParams> : IAsyncDisposable
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <summary>
    /// Stable ID identifying this logical transaction in the resiliency layer.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Signed submission attempts for this transaction on its active nonce, ordered newest first.
    /// </summary>
    public ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions { get; }

    /// <summary>
    /// Persists the latest submission if it has not yet been persisted, broadcasts the latest signed payload
    /// through the configured publisher, and returns immediately without waiting for inclusion.
    /// Safe to call repeatedly to rebroadcast the latest submission.
    /// </summary>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Token used to cancel the publish request.</param>
    /// <returns>The immediate broadcast outcome.</returns>
    public Task<TxSubmissionResult> PublishAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Signs a new submission at the same nonce with updated gas parameters and registers its hash with
    /// the active watcher. Does not persist or broadcast.
    /// </summary>
    /// <param name="gasParams">Updated gas parameters for the replacement submission.</param>
    /// <param name="cancellationToken">Token used to cancel signing.</param>
    /// <returns>
    /// The newly created submission, or <see langword="null"/> if this transaction's nonce has already resolved on-chain.
    /// </returns>
    public Task<TxSubmission<TTxParams, TTxGasParams>?> TryRepriceAsync(
        TTxGasParams gasParams,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Waits for this transaction's nonce slot to resolve on-chain as confirmed or replaced.
    /// Cancelling <paramref name="cancellationToken"/> only cancels this wait operation.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel waiting.</param>
    /// <returns>The inclusion outcome for this transaction's nonce slot.</returns>
    public Task<TxWatchResult> WaitForInclusionAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Waits up to <paramref name="timeout"/> for this transaction's nonce slot to resolve on-chain as confirmed or replaced.
    /// Timing out or cancelling <paramref name="cancellationToken"/> only ends this wait operation.
    /// </summary>
    /// <param name="timeout">Maximum duration to wait before returning <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token used to cancel waiting.</param>
    /// <returns>
    /// The inclusion outcome for this transaction's nonce slot, or <see langword="null"/> if <paramref name="timeout"/> elapsed.
    /// </returns>
    public Task<TxWatchResult?> WaitForInclusionAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Waits for inclusion and, if one of this transaction's submissions was included, fetches its receipt.
    /// Cancelling <paramref name="cancellationToken"/> only cancels this wait operation.
    /// </summary>
    /// <param name="requestOptions">Transport-specific request options used when fetching the receipt.</param>
    /// <param name="cancellationToken">Token used to cancel waiting or receipt retrieval.</param>
    /// <returns>The receipt outcome for this transaction.</returns>
    public Task<TxReceiptResult> WaitForReceiptAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Waits up to <paramref name="timeout"/> for inclusion and, if one of this transaction's submissions was included, fetches its receipt.
    /// Timing out or cancelling <paramref name="cancellationToken"/> only ends this wait operation.
    /// </summary>
    /// <param name="timeout">Maximum duration to wait before returning <see langword="null"/>.</param>
    /// <param name="requestOptions">Transport-specific request options used when fetching the receipt.</param>
    /// <param name="cancellationToken">Token used to cancel waiting or receipt retrieval.</param>
    /// <returns>
    /// The receipt outcome for this transaction, or <see langword="null"/> if <paramref name="timeout"/> elapsed.
    /// </returns>
    public Task<TxReceiptResult?> WaitForReceiptAsync(
        TimeSpan timeout,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );
}
