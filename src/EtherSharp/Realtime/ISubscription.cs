namespace EtherSharp.Realtime;

public interface ISubscription : IAsyncDisposable
{
    public string Id { get; }
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload);
    public Task InstallAsync(CancellationToken cancellationToken = default);
}
