using EtherSharp.RPC.Transport;

namespace EtherSharp.Realtime.Events.Filter;

/// <summary>
/// Represents a polling-based event filter that returns decoded log changes.
/// </summary>
/// <typeparam name="TLog">The decoded log type.</typeparam>
public interface IEventFilter<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    /// <summary>
    /// Gets new event log changes since the previous poll.
    /// </summary>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">Token used to cancel the poll request.</param>
    /// <returns>The decoded log changes.</returns>
    public Task<TLog[]> GetChangesAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);
}
