using EtherSharp.RPC.Transport;

namespace EtherSharp.Client.Services.TxPublisher;

/// <summary>
/// Publishes signed transaction payloads to an Ethereum-compatible network.
/// </summary>
public interface ITxPublisher : IAsyncDisposable
{
    /// <summary>
    /// Submits a signed transaction to the network and returns submission details.
    /// </summary>
    /// <param name="transactionHex">The raw signed transaction encoded as a hexadecimal string.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">A token used to cancel the publish request.</param>
    /// <returns>The submission result including status and transaction hash information.</returns>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> was cancelled. Implementations must throw rather than return a result.
    /// </exception>
    public Task<TxSubmissionResult> PublishTxAsync(
        string transactionHex, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );
}
