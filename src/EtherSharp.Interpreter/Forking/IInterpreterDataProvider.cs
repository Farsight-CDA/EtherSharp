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
    /// <returns>
    /// Self-identifying results for requested values and any additional values prefetched by the provider.
    /// </returns>
    /// <remarks>
    /// Partial batches are allowed; the fork caches results and retries unanswered requests.
    /// Each response must resolve at least one pending read, or dispatched reads fail.
    /// </remarks>
    public Task<IReadOnlyList<InterpreterDataResult>> FetchAsync(
        InterpreterContext context,
        ReadOnlyMemory<InterpreterDataRequest> requests
    );
}
