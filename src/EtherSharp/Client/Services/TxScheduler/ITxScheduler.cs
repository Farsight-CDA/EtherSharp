using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.TxScheduler;
public interface ITxScheduler
{
    public Task<TransactionReceipt> PublishTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput txInput, TTxParams? txParams, TTxGasParams? txGasParams,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;
}
