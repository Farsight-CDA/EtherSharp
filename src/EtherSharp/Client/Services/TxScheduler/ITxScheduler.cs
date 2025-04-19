using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxScheduler;
public interface ITxScheduler
{
    public ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
         ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default
     )
         where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
         where TTxParams : class, ITxParams<TTxParams>
         where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    public ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
