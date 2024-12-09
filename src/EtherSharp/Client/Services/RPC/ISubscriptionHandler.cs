namespace EtherSharp.Client.Services.RPC;
public interface ISubscriptionHandler<TPayload>
{
    public Task<string> InstallAsync();
    public void HandlePayload(TPayload payload);
}
