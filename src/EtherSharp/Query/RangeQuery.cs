namespace EtherSharp.Query;

internal sealed record RangeQuery<T>(
    ReadOnlyMemory<IQuery<T>> Queries
) : IQuery<T[]>
{
    public int OperationCount
    {
        get {
            int operationCount = 0;
            foreach(var query in Queries.Span)
            {
                operationCount += query.OperationCount;
            }

            return operationCount;
        }
    }

    void IQuery<T[]>.AddTo(QueryPlan plan)
    {
        foreach(var query in Queries.Span)
        {
            plan.Add(query);
        }
    }

    T[] IQuery<T[]>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var results = new T[Queries.Length];
        int offset = 0;

        for(int i = 0; i < Queries.Length; i++)
        {
            var query = Queries.Span[i];
            int count = query.OperationCount;
            results[i] = query.ReadResultFrom(queryResults.Slice(offset, count));
            offset += count;
        }

        return results;
    }
}
