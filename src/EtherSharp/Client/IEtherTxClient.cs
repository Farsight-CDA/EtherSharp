using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public new IEtherTxApi ETH { get; }

    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call, Func<ValueTask<TxTimeoutAction>> onTxTimeout, 
        EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default);
    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call, Func<TxTimeoutAction> onTxTimeout,
        EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default);

    public Task<TransactionReceipt> ExecuteTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, Func<ValueTask<TxTimeoutAction>> onTxTimeout,
        TTxParams? txParams = default, TTxGasParams? txGasParams = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;

    public Task<TransactionReceipt> ExecuteTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, Func<TxTimeoutAction> onTxTimeout,
        TTxParams? txParams = default, TTxGasParams? txGasParams = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;
}
