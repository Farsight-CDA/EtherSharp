﻿namespace EtherSharp.Client.Services.RPC;
public interface IRpcMiddleware
{
    public Task<RpcResult<TResult>> HandleAsync<TResult>(CancellationToken cancellationToken, Func<CancellationToken, Task<RpcResult<TResult>>> onNext);
}
