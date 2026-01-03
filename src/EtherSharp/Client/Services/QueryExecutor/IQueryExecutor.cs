using EtherSharp.Query;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.QueryExecutor;

public interface IQueryExecutor
{
    public Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetHeight, CancellationToken cancellationToken);
}
