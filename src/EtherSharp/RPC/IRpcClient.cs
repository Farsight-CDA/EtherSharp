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
    /// Sends a JSON-RPC request with explicit parameter list.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a JSON-RPC request with no parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [], requiredBlockNumber, cancellationToken);

    /// <summary>
    /// Sends a JSON-RPC request with one parameter.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1], requiredBlockNumber, cancellationToken);

    /// <summary>
    /// Sends a JSON-RPC request with two parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2], requiredBlockNumber, cancellationToken);

    /// <summary>
    /// Sends a JSON-RPC request with three parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2, t3], requiredBlockNumber, cancellationToken);

    /// <summary>
    /// Sends a JSON-RPC request with four parameters.
    /// </summary>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, T4, TResult>(
        string method, T1 t1, T2 t2, T3 t3, T4 t4, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2, t3, t4], requiredBlockNumber, cancellationToken);
}
