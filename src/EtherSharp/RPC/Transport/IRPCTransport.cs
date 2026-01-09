using EtherSharp.Types;

namespace EtherSharp.RPC.Transport;
/// <summary>
/// Common interface for all RPC Transports to implement.
/// </summary>
public interface IRPCTransport
{
    /// <summary>
    /// Specifies if the transport supports filters.
    /// </summary>
    public bool SupportsFilters { get; }
    /// <summary>
    /// Specifies if the transport supports subscriptions.
    /// </summary>
    public bool SupportsSubscriptions { get; }

    /// <summary>
    /// Fires when the underlying connection has been established.
    /// </summary>
    public event Action? OnConnectionEstablished;

    /// <summary>
    /// Fires when a subscription message has been received.
    /// </summary>
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    /// <summary>
    /// Initializes this RPCTransport.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to this RPCTransport and returns its response.
    /// </summary>
    /// <typeparam name="TResult">Expected result type.</typeparam>
    /// <param name="method">Rpc method name.</param>
    /// <param name="parameters">List of parameters for this call.</param>
    /// <param name="requiredBlockNumber">The minimum target block required by the node in order to fulfil this request.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method,
        object?[] parameters,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default
    );
}
