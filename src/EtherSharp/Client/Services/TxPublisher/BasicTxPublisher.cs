using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Client.Services.TxPublisher;
public class BasicTxPublisher(IRpcClient rpcClient) : ITxPublisher
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public Task<TxSubmissionResult> PublishTxAsync(string transactionHex)
        => _rpcClient.EthSendRawTransactionAsync(transactionHex);
}
