using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;

namespace EtherSharp.Client.Services.TxPublisher;

/// <summary>
/// Publishes signed transactions through the configured Ethereum RPC module.
/// </summary>
/// <param name="ethRpcModule">RPC module used to submit raw transactions.</param>
public sealed class BasicTxPublisher(IEthRpcModule ethRpcModule) : ITxPublisher
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    /// <inheritdoc/>
    public async Task<TxSubmissionResult> PublishTxAsync(
        string transactionHex, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _ethRpcModule.SendRawTransactionAsync(transactionHex, requestOptions, cancellationToken);
        }
        catch(Exception ex)
        {
            return TxSubmissionResult.FromException(ex);
        }
    }
}
