namespace EtherSharp.Client.Services.RPC;
public interface IRpcMiddleware
{
    public Task HandleAsync(Func<Task> onNext);
}
