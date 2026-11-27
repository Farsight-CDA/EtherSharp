using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client;

/// <summary>
/// Transaction-capable Ethereum client abstraction extending <see cref="IEtherClient"/>.
/// </summary>
/// <remarks>
/// Disposing only releases signer-scoped resources, not the underlying <see cref="IEtherClient"/>.
/// Disposing the underlying client disposes all attached tx clients.
/// </remarks>
public interface IEtherTxClient : IEtherClient
{
    /// <summary>
    /// Gets the address of the signer attached to this transaction client.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Prepares and signs a default EIP-1559 transaction without broadcasting or persisting it.
    /// </summary>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (access list, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending transaction handle for publishing, repricing, and awaiting confirmation.</returns>
    public Task<IPendingTx<EIP1559TxParams, EIP1559GasParams>> PrepareTxAsync(
        ITxInput call,
        EIP1559TxParams? txParams = default,
        EIP1559GasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => PrepareTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
        call, txParams, txGasParams, requestOptions, cancellationToken
    );

    /// <summary>
    /// Prepares and signs a transaction for a custom transaction model without broadcasting or persisting it.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters.</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending transaction handle for publishing, repricing, and awaiting confirmation.</returns>
    public Task<IPendingTx<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call,
        TTxParams? txParams = default,
        TTxGasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    /// <summary>
    /// Prepares, signs and publishes a default EIP-1559 transaction once, then waits for its receipt.
    /// Never reprices or rebroadcasts; use <see cref="PrepareTxAsync"/> for control over the submission lifecycle.
    /// </summary>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (access list, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The receipt outcome for the transaction.</returns>
    /// <exception cref="TxPublishException">The node rejected the transaction.</exception>
    public Task<TxReceiptResult> SendTxAndWaitForReceiptAsync(
        ITxInput call,
        EIP1559TxParams? txParams = default,
        EIP1559GasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => SendTxAndWaitForReceiptAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
        call, txParams, txGasParams, requestOptions, cancellationToken
    );

    /// <summary>
    /// Prepares, signs and publishes a transaction for a custom transaction model once, then waits for its receipt.
    /// Never reprices or rebroadcasts; use <see cref="PrepareTxAsync{TTransaction, TTxParams, TTxGasParams}"/>
    /// for control over the submission lifecycle.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters.</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The receipt outcome for the transaction.</returns>
    /// <exception cref="TxPublishException">The node rejected the transaction.</exception>
    /// <remarks>
    /// A <see cref="TxSubmissionResult.NonceTooLow"/> publish result is not treated as a failure, as the nonce
    /// may have been consumed by this transaction itself. The returned result reports the actual outcome.
    /// </remarks>
    public async Task<TxReceiptResult> SendTxAndWaitForReceiptAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call,
        TTxParams? txParams = default,
        TTxGasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        await using var pendingTx = await PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
            call, txParams, txGasParams, requestOptions, cancellationToken
        );

        var submissionResult = await pendingTx.PublishAsync(requestOptions, cancellationToken);
        if(submissionResult.IsAccepted || submissionResult is TxSubmissionResult.NonceTooLow)
        {
            return await pendingTx.WaitForReceiptAsync(requestOptions, cancellationToken);
        }

        _ = await pendingTx.TryAbandonAsync(CancellationToken.None);
        throw new TxPublishException(submissionResult);
    }

    /// <summary>
    /// Attaches to a persisted default EIP-1559 transaction by ID for this client's chain and sender.
    /// </summary>
    /// <param name="id">The pending transaction's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A pending transaction handle bound to the existing transaction, or <see langword="null"/> if no persisted submissions exist for <paramref name="id"/>.
    /// </returns>
    public Task<IPendingTx<EIP1559TxParams, EIP1559GasParams>?> AttachPendingTxAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => AttachPendingTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(id, cancellationToken);

    /// <summary>
    /// Attaches to a persisted transaction by ID for this client's chain and sender using custom transaction model types.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="id">The pending transaction's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A pending transaction handle bound to the existing transaction, or <see langword="null"/> if no persisted submissions exist for <paramref name="id"/>.
    /// </returns>
    public Task<IPendingTx<TTxParams, TTxGasParams>?> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id,
        CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
