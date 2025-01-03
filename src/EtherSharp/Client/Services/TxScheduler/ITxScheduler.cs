using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.TxScheduler;
public interface ITxScheduler
{
    public Task<TransactionReceipt> PublishTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput txInput, ITxParams txParams, ITxGasParams? txGasParams,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams;
}
