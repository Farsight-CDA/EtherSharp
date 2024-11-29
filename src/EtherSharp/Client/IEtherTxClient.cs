using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

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
}
