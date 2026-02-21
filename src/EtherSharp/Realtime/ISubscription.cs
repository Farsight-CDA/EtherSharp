namespace EtherSharp.Realtime;

/// <summary>
/// Represents a low-level realtime RPC subscription that can be installed and fed with incoming payloads.
/// </summary>
public interface ISubscription : IAsyncDisposable
{
    public string Id { get; }
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload);
    public Task InstallAsync(CancellationToken cancellationToken = default);
}
