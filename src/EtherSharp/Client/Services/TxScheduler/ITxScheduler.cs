using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.TxScheduler;
public interface ITxScheduler
{
    public Task<TransactionReceipt> PublishTxAsync<TTxParams>(
        TTxParams txParams, ITxInput txInput,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    ) where TTxParams : ITxParams;
}
