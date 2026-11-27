using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using System.Collections.Immutable;

namespace EtherSharp.Tx;

/// <summary>
/// Represents a prepared or in-flight logical transaction with single-step publish, reprice, and confirmation operations.
/// </summary>
/// <remarks>
/// Disposing cancels in-flight operations and stops local tracking, but never rolls back:
/// the nonce stays allocated, persisted submissions are kept and broadcast transactions may still be included.
/// </remarks>
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
    /// Most recent signed submission attempt for this transaction on its active nonce.
    /// </summary>
    public TxSubmission<TTxParams, TTxGasParams> LatestSubmission { get; }

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
    /// Signs a replacement at the same nonce using the latest fees scaled by 1105/1000 (a 10.5% bump),
    /// with a minimum increase of 1 wei per gas. Does not persist or broadcast.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel signing.</param>
    /// <returns>The signed replacement, or <see langword="null"/> if the nonce has already resolved on-chain.</returns>
    public Task<TxSubmission<TTxParams, TTxGasParams>?> TryBumpFeesAsync(CancellationToken cancellationToken = default)
        => TryBumpFeesAsync(1105, 1000, 1, cancellationToken);

    /// <summary>
    /// Signs a replacement at the same nonce by scaling the latest submission's fees.
    /// Registers the replacement hash with the active watcher without persisting or broadcasting.
    /// </summary>
    /// <param name="multiplier">Numerator of the fee scaling factor, at least <paramref name="divider"/>.</param>
    /// <param name="divider">Nonzero denominator of the fee scaling factor.</param>
    /// <param name="minimumIncrement">Minimum absolute increase in each fee per gas.</param>
    /// <param name="cancellationToken">Token used to cancel signing.</param>
    /// <returns>The signed replacement, or <see langword="null"/> if the nonce has already resolved on-chain.</returns>
    public Task<TxSubmission<TTxParams, TTxGasParams>?> TryBumpFeesAsync(
        UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement,
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
