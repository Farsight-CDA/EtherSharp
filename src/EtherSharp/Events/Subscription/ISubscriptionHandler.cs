namespace EtherSharp.Events.Subscription;
public interface ISubscriptionHandler<TPayload>
{
    public Task<string> InstallAsync();
    public void HandlePayload(TPayload payload);
}
