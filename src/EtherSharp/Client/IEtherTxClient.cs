using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public new IEtherTxApi ETH { get; }

    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    );

    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call,
        Func<TxTimeoutAction> onTxTimeout
    );

    public Task<TransactionReceipt> ExecuteTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams txParams, TTxGasParams gasParams,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams;

    public Task<TransactionReceipt> ExecuteTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams txParams, TTxGasParams gasParams,
        Func<TxTimeoutAction> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams;

    public Task<TransactionReceipt> ExecuteTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams txParams,
        Func<TxTimeoutAction> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams;

    public Task<TransactionReceipt> ExecuteTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams txParams,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams;
}
