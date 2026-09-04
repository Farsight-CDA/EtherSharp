namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Resolves batches of logical interpreter data requests against an upstream state snapshot.
/// </summary>
public interface IInterpreterDataProvider
{
    /// <summary>
    /// Resolves a batch of requests at the supplied interpreter context.
    /// </summary>
    /// <param name="context">The state and block context against which the requests are resolved.</param>
    /// <param name="requests">The requests to resolve.</param>
    /// <returns>
    /// Self-identifying results for every requested value and any additional values prefetched by the provider.
    /// </returns>
    public Task<InterpreterDataResult[]> FetchAsync(
        InterpreterContext context,
        ReadOnlyMemory<InterpreterDataRequest> requests
    );
}
