using EtherSharp.Types;

namespace EtherSharp.RPC;

/// <summary>
/// Sends JSON-RPC requests through the configured transport.
/// </summary>
public interface IRpcClient
{
    /// <summary>
    /// Raised when the underlying transport connection is established.
    /// </summary>
    public event Action? OnConnectionEstablished;

    /// <summary>
    /// Raised when a subscription payload is received.
    /// </summary>
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    /// <summary>
    /// Gets whether filter methods are supported.
    /// </summary>
    public bool SupportsFilters { get; }

    /// <summary>
    /// Gets whether pub/sub methods are supported.
    /// </summary>
    public bool SupportsSubscriptions { get; }

    /// <summary>
    /// Sends a JSON-RPC request with no parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a JSON-RPC request with one parameter.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a JSON-RPC request with two parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a JSON-RPC request with three parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetHeight requiredBlockNumber, CancellationToken cancellationToken = default
    );
}
