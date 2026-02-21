using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxScheduler;

/// <summary>
/// Coordinates transaction preparation and tracking for a client instance.
/// </summary>
public interface ITxScheduler
{
    /// <summary>
    /// Prepares a new pending transaction handler from the provided transaction input and optional parameters.
    /// </summary>
    /// <typeparam name="TTransaction">The transaction model type that defines encoding and behavior.</typeparam>
    /// <typeparam name="TTxParams">The transaction parameter type associated with <typeparamref name="TTransaction"/>.</typeparam>
    /// <typeparam name="TTxGasParams">The gas parameter type associated with <typeparamref name="TTransaction"/>.</typeparam>
    /// <param name="call">The transaction input payload to prepare.</param>
    /// <param name="txParams">Optional transaction parameters; defaults are resolved when not provided.</param>
    /// <param name="txGasParams">Optional gas parameters; estimators may populate missing values.</param>
    /// <param name="cancellationToken">A token used to cancel preparation.</param>
    /// <returns>A pending transaction handler that can submit, monitor, and manage the transaction lifecycle.</returns>
    public ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
         ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default, CancellationToken cancellationToken = default
     )
         where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
         where TTxParams : class, ITxParams<TTxParams>
         where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    /// <summary>
    /// Reattaches to an already-submitted transaction identified by nonce.
    /// </summary>
    /// <typeparam name="TTransaction">The transaction model type that defines encoding and behavior.</typeparam>
    /// <typeparam name="TTxParams">The transaction parameter type associated with <typeparamref name="TTransaction"/>.</typeparam>
    /// <typeparam name="TTxGasParams">The gas parameter type associated with <typeparamref name="TTransaction"/>.</typeparam>
    /// <param name="nonce">The nonce of the transaction to recover and track.</param>
    /// <param name="cancellationToken">A token used to cancel recovery.</param>
    /// <returns>A pending transaction handler attached to the existing on-chain submission flow.</returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        uint nonce, CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
