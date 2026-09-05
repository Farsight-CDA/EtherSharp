using EtherSharp.Interpreter.Runtime;

namespace EtherSharp.Interpreter.Forking;

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
    /// Self-identifying results for requested values and any additional values prefetched by the provider.
    /// </returns>
    /// <remarks>
    /// Providers may limit each fetch to their source's batch capacity. The fork retains returned values,
    /// completes satisfied reads, and includes unanswered requests in the next batch. A response must
    /// resolve at least one pending read to allow execution to resume; otherwise dispatched reads fail.
    /// </remarks>
    public Task<InterpreterDataResult[]> FetchAsync(
        InterpreterContext context,
        ReadOnlyMemory<InterpreterDataRequest> requests
    );
}
