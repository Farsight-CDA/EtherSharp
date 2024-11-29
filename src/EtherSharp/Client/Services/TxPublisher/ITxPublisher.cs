using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Client.Services.TxPublisher;
public interface ITxPublisher
{
    public Task<TxSubmissionResult> PublishTxAsync(string transactionHex);

}
