using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Transport;
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

    public ValueTask InitializeAsync(CancellationToken cancellationToken = default);

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken = default);
}
