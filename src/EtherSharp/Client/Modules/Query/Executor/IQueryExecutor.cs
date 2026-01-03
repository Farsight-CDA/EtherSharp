using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query.Executor;

public interface IQueryExecutor
{
    public Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetHeight, CancellationToken cancellationToken);
}
