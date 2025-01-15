using EtherSharp.Types;

namespace EtherSharp.Client.Services.TxPublisher;
public interface ITxPublisher
{
    public Task<TxSubmissionResult> PublishTxAsync(string transactionHex);

}
