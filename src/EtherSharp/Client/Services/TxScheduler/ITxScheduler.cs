using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxScheduler;
public interface ITxScheduler
{
    public Task<string> PublishTxAsync(TxType txType, ITxInput txInput);
}
