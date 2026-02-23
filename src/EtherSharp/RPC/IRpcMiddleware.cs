namespace EtherSharp.RPC;

/// <summary>
/// Defines a middleware component in the RPC request pipeline.
/// </summary>
public interface IRpcMiddleware
{
    /// <summary>
    /// Handles an RPC request and either forwards execution to the next middleware or returns a result.
    /// </summary>
    /// <typeparam name="TResult">The response payload type.</typeparam>
    /// <param name="onNext">The delegate that invokes the next middleware in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    /// <returns>The RPC result produced by this middleware or a downstream middleware.</returns>
    public Task<RpcResult<TResult>> HandleAsync<TResult>(Func<CancellationToken, Task<RpcResult<TResult>>> onNext, CancellationToken cancellationToken);
}
