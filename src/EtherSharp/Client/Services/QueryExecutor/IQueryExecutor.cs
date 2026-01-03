using EtherSharp.Query;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.QueryExecutor;
/// <summary>
/// Interface for query executor strategies.
/// </summary>
public interface IQueryExecutor
{
    /// <summary>
    /// Executes the given query against the blockchain.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <param name="query"></param>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetHeight, CancellationToken cancellationToken);
}
