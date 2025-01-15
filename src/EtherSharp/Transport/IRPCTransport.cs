using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Transport;
public interface IRPCTransport
{
    public bool SupportsFilters { get; }
    public bool SupportsSubscriptions { get; }

    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public ValueTask InitializeAsync(CancellationToken cancellationToken = default);

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken = default);
}
