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
    /// Handles the result of an incoming subscription notification.
    /// </summary>
    /// <param name="result">The raw JSON of the notification result, never <c>null</c>.</param>
    public void HandleSubscriptionMessage(ReadOnlySpan<byte> result);

    /// <summary>
    /// Reports an interruption to listeners. Further interruptions are ignored until the next <see cref="Resume"/>.
    /// </summary>
    /// <param name="interruption">The interruption that stopped the subscription from receiving notifications.</param>
    public void Interrupt(SubscriptionInterrupted interruption);

    /// <summary>
    /// Called once the subscription is live again, before any of its notifications. Reports the resumption of an interrupted subscription to listeners.
    /// </summary>
    public void Resume();

    /// <summary>
    /// Installs the subscription on the remote node. Called again with the same options to reinstall it after an interruption.
    /// </summary>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Token used to cancel the installation request.</param>
    public Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the local subscription state without sending an unsubscribe RPC, ending the stream of listeners.
    /// </summary>
    public void Close();
}
