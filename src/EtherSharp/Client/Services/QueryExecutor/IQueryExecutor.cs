using EtherSharp.Query;

namespace EtherSharp.Client.Services.QueryExecutor;

/// <summary>
/// Executes typed read-only queries against the blockchain.
/// </summary>
public interface IQueryExecutor
{
    /// <summary>
    /// Executes a query at the requested block height and returns the decoded query result.
    /// </summary>
    /// <typeparam name="TQuery">The query result type produced by the provided query object.</typeparam>
    /// <param name="query">The query definition containing call data and decode logic.</param>
    /// <param name="gasLimit">The optional gas cap applied to the configured query execution strategy.</param>
    /// <param name="options">The call execution context used for the query.</param>
    /// <param name="cancellationToken">A token used to cancel the underlying RPC call.</param>
    /// <returns>The decoded result for the supplied query.</returns>
    public Task<TQuery> ExecuteQueryAsync<TQuery>(
        IQuery<TQuery> query,
        ulong? gasLimit,
        CallOptions options,
        CancellationToken cancellationToken
    );
}
