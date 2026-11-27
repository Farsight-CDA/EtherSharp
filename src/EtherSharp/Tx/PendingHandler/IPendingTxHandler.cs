using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using System.Collections.Immutable;

namespace EtherSharp.Tx.PendingHandler;

/// <summary>
/// Manages the submission and confirmation of a pending transaction.
/// </summary>
/// <typeparam name="TTxParams"></typeparam>
/// <typeparam name="TTxGasParams"></typeparam>
public interface IPendingTxHandler<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <summary>
    /// Stable ID used to recover this transaction and its replacement attempts from the resiliency layer.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Submission attempts for this transaction.
    /// </summary>
    public ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions { get; }

    /// <summary>
    /// Publishes the most recent tx submission to the mempool and waits for it to be confirmed on the blockchain.
    /// Provides a callback to handle errors and timeouts.
    /// </summary>
    /// <param name="onError"></param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <returns></returns>
    public Task<TxConfirmationResult> PublishAndConfirmAsync(
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError,
        RpcRequestOptions requestOptions = default
    );
}
