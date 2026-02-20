using EtherSharp.Client.Modules.Ether;
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
    /// Gets the transaction-capable ETH module.
    /// </summary>
    public new IEtherTxModule ETH { get; }

    /// <summary>
    /// Creates a pending-transaction handler for a default EIP-1559 transaction.
    /// </summary>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (nonce, value, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <returns>A pending-transaction handler for submission and lifecycle control.</returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> PrepareTxAsync(ITxInput call, EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default)
        => PrepareTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(call, txParams, txGasParams);

    /// <summary>
    /// Creates a pending-transaction handler for a custom transaction model.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="call">Transaction call payload.</param>
    /// <param name="txParams">Optional transaction parameters (nonce, value, etc.).</param>
    /// <param name="txGasParams">Optional precomputed gas parameters.</param>
    /// <returns>A pending-transaction handler for submission and lifecycle control.</returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    /// <summary>
    /// Attaches to an already-pending default EIP-1559 transaction by nonce.
    /// </summary>
    /// <param name="nonce">Sender nonce of the pending transaction.</param>
    /// <returns>A pending-transaction handler bound to the existing transaction.</returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> AttachPendingTxAsync(uint nonce)
        => AttachPendingTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(nonce);

    /// <summary>
    /// Attaches to an already-pending transaction by nonce using custom transaction model types.
    /// </summary>
    /// <typeparam name="TTransaction">Transaction type implementing signing/serialization behavior.</typeparam>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
    /// <param name="nonce">Sender nonce of the pending transaction.</param>
    /// <returns>A pending-transaction handler bound to the existing transaction.</returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
