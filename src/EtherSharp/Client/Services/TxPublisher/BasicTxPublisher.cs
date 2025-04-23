using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Client.Services.TxPublisher;
public class BasicTxPublisher(IRpcClient rpcClient) : ITxPublisher
{
    private readonly IRpcClient _rpcClient = rpcClient;

    /// <inheritdoc/>
    public async Task<TxSubmissionResult> PublishTxAsync(string transactionHex, CancellationToken cancellationToken)
    {
        try
        {
            return await _rpcClient.EthSendRawTransactionAsync(transactionHex, cancellationToken);
        }
        catch(Exception ex)
        {
            return new TxSubmissionResult.UnhandledException(ex);
        }
    }
}
