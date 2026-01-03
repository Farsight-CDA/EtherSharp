using EtherSharp.Client.Modules.Ether;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client;

public interface IEtherTxClient : IEtherClient
{
    public new IEtherTxModule ETH { get; }

    /// <summary>
    /// Creates a handler for submitting a transaction.
    /// </summary>
    /// <param name="call"></param>
    /// <param name="txParams"></param>
    /// <param name="txGasParams"></param>
    /// <returns></returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> PrepareTxAsync(ITxInput call, EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default)
        => PrepareTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(call, txParams, txGasParams);

    /// <summary>
    /// Creates a handler for submitting a transaction.
    /// </summary>
    /// <typeparam name="TTransaction"></typeparam>
    /// <typeparam name="TTxParams"></typeparam>
    /// <typeparam name="TTxGasParams"></typeparam>
    /// <param name="call"></param>
    /// <param name="txParams"></param>
    /// <param name="txGasParams"></param>
    /// <returns></returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    /// <summary>
    /// Attach to a pending transaction with the given nonce.
    /// </summary>
    /// <param name="nonce"></param>
    /// <returns></returns>
    public Task<IPendingTxHandler<EIP1559TxParams, EIP1559GasParams>> AttachPendingTxAsync(uint nonce)
        => AttachPendingTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(nonce);

    /// <summary>
    /// Attach to a pending transaction with the given nonce.
    /// </summary>
    /// <typeparam name="TTransaction"></typeparam>
    /// <typeparam name="TTxParams"></typeparam>
    /// <typeparam name="TTxGasParams"></typeparam>
    /// <param name="nonce"></param>
    /// <returns></returns>
    public Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
