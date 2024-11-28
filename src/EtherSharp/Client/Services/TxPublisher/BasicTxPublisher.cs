using EtherSharp.RPC;

namespace EtherSharp.Client.Services.TxPublisher;
public class BasicTxPublisher(EvmRpcClient rpcClient) : ITxPublisher
{
    private readonly EvmRpcClient _rpcClient = rpcClient;

    public Task<string> PublishTxAsync(string transactionHex)
        => _rpcClient.EthSendRawTransactionAsync(transactionHex);
}
