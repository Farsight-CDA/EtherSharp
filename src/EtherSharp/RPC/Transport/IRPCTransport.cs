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
    /// Sends a request without parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a request with one parameter.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method,
        T1 t1,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a request with two parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method,
        T1 t1,
        T2 t2,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a request with three parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method,
        T1 t1,
        T2 t2,
        T3 t3,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default
    );
}
