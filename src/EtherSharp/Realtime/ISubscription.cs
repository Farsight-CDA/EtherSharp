namespace EtherSharp.Realtime;

public interface ISubscription
{
    public string Id { get; }
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload);
    public Task InstallAsync(CancellationToken cancellationToken = default);
}
