namespace EtherSharp.RPC;
public interface IRpcMiddleware
{
    public Task<RpcResult<TResult>> HandleAsync<TResult>(Func<CancellationToken, Task<RpcResult<TResult>>> onNext, CancellationToken cancellationToken);
}
