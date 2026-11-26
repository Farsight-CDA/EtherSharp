using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client;

/// <summary>
/// Transaction-capable Ethereum client abstraction extending <see cref="IEtherClient"/>.
/// </summary>
public interface IEtherTxClient : IEtherClient
{
    /// <summary>
    /// Creates a pending-transaction handler for a default EIP-1559 transaction.
    /// </summary>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (nonce, value, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending-transaction handler for submission and lifecycle control.</returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> PrepareTxAsync(
        ITxInput call, EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    ) => PrepareTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
            call, txParams, txGasParams, requestOptions, cancellationToken
        );

    /// <summary>
    /// Creates a pending-transaction handler for a custom transaction model.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (nonce, value, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending-transaction handler for submission and lifecycle control.</returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    /// <summary>
    /// Attaches to a persisted default EIP-1559 transaction by ID for this client's chain and sender.
    /// </summary>
    /// <param name="id">The pending transaction handler's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending-transaction handler bound to the existing transaction.</returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> AttachPendingTxAsync(
        Guid id, CancellationToken cancellationToken = default
    ) => AttachPendingTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(id, cancellationToken);

    /// <summary>
    /// Attaches to a persisted transaction by ID for this client's chain and sender using custom transaction model types.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="id">The pending transaction handler's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A pending-transaction handler bound to the existing transaction.</returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
