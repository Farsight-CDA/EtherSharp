using EtherSharp.RPC.Transport;

namespace EtherSharp.Realtime;

/// <summary>
/// Represents a low-level realtime RPC subscription that can be installed and fed with incoming payloads.
/// </summary>
public interface ISubscription : IAsyncDisposable
{
    /// <summary>
    /// Gets the current remote subscription identifier assigned by the node.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the transport-specific options used to install the subscription.
    /// </summary>
    public RpcRequestOptions RequestOptions { get; }

    /// <summary>
    /// Handles an incoming subscription payload.
    /// </summary>
    /// <param name="payload">The raw subscription payload bytes.</param>
    /// <returns><see langword="true"/> when the payload was consumed by this subscription; otherwise, <see langword="false"/>.</returns>
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload);

    /// <summary>
    /// Installs the subscription on the remote node.
    /// </summary>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Token used to cancel the installation request.</param>
    public Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the local subscription state without sending an unsubscribe RPC.
    /// </summary>
    public void Close();
}
