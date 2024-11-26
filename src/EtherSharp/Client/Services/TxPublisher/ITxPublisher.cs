namespace EtherSharp.Client.Services.TxPublisher;
public interface ITxPublisher
{
    public Task<string> PublishTxAsync(string transactionHex);

}
