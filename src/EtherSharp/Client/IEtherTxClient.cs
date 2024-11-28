using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call, 
        Func<ValueTask<TxTimeoutAction>> onTxTimeout  
    );

    public Task<TransactionReceipt> ExecuteTxAsync(ITxInput call,
        Func<TxTimeoutAction> onTxTimeout
    );
}
