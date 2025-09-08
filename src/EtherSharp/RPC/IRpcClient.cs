namespace EtherSharp.RPC;
public interface IRpcClient
{
    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public bool SupportsFilters { get; }
    public bool SupportsSubscriptions { get; }

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken);

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2, t3], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, T4, TResult>(
        string method, T1 t1, T2 t2, T3 t3, T4 t4, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2, t3, t4], cancellationToken);
}
