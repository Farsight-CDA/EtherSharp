namespace EtherSharp.Client.Services.RPC;
public interface IRpcMiddleware
{
    public Task<RpcResult<TResult>> HandleAsync<TResult>(Func<Task<RpcResult<TResult>>> onNext);
}
