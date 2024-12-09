namespace EtherSharp.Client.Services.RPC;
public interface ISubscriptionHandler<TPayload>
{
    public Task<string> InstallAsync(IRpcClient client);
    public void HandlePayload(TPayload payload);
}
