namespace EtherSharp.Client.Modules.Query;

public partial class QueryBuilder<TQuery> : IQuery<List<TQuery>>, IQueryBuilder<TQuery>
{
    private readonly List<IQuery> _queries = [];
    private readonly List<(int, Func<ReadOnlySpan<byte[]>, TQuery>)> _resultSelectorFunctions = [];

    IReadOnlyList<IQuery> IQueryBuilder<TQuery>.Queries => _queries;
    IReadOnlyList<IQuery> IQuery<List<TQuery>>.Queries => _queries;

    List<TQuery> IQueryBuilder<TQuery>.ParseResults(byte[][] outputs)
    {
        var results = new List<TQuery>(_resultSelectorFunctions.Count);

        foreach(var (offset, selector) in _resultSelectorFunctions)
        {
            results.Add(selector.Invoke(outputs.AsSpan(offset)));
        }

        return results;
    }

    List<TQuery> IQuery<List<TQuery>>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults)
    {
        var results = new List<TQuery>();
        foreach(var (offset, selectorFunc) in _resultSelectorFunctions)
        {
            results.Add(selectorFunc(queryResults[offset..]));
        }
        return results;
    }

    public QueryBuilder<TQuery> AddQuery(IQuery<TQuery> c1)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => c1.ReadResultFrom(x[o1..])));
        return this;
    }

    public QueryBuilder<TQuery> AddQueries(params ReadOnlySpan<IQuery<TQuery>> queries)
    {
        foreach(var query in queries)
        {
            AddQuery(query);
        }

        return this;
    }

    public QueryBuilder<TQuery> AddQueries<T1>(Func<T1, TQuery> mapping, params ReadOnlySpan<IQuery<T1>> queries)
    {
        foreach(var query in queries)
        {
            AddQuery(query, mapping);
        }

        return this;
    }
}