namespace EtherSharp.Query;

internal sealed record MappedQuery<TFrom, TTo>(
    IQuery<TFrom> Query,
    Func<TFrom, TTo> Mapping
) : IQuery<TTo>
{
    public int OperationCount
        => Query.OperationCount;

    void IQuery<TTo>.AddTo(QueryPlan plan)
        => plan.Add(Query);

    TTo IQuery<TTo>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => Mapping(Query.ReadResultFrom(queryResults));
}
