namespace EtherSharp.Client.Services.TxPublisher;
public class BasicTxPublisher : ITxPublisher
{
    public Task<string> PublishTxAsync(string transactionHex)
        => throw new NotImplementedException();
}
