using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Transport;
public interface IRPCTransport
{
    public bool SupportsFilters { get; }
    public bool SupportsSubscriptions { get; }
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public ValueTask InitializeAsync(CancellationToken cancellationToken = default);

    public Task<RpcResult<TResult>> SendRpcRequest<TResult>(
        string method, CancellationToken cancellationToken = default
    );
    public Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(
        string method, T1 t1, CancellationToken cancellationToken = default
    );
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(
        string method, T1 t1, T2 t2, CancellationToken cancellationToken = default
    );
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken = default
    );
}
