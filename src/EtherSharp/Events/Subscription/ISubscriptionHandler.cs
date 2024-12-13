namespace EtherSharp.Events.Subscription;
public interface ISubscriptionHandler<TPayload>
{
    public Task<string> InstallAsync(CancellationToken cancellationToken);
    public void HandlePayload(TPayload payload);
}
