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

    public QueryBuilder<TQuery> AddQuery(IQuery<TQuery> c)
    {
        int resultIndex = _queries.Count;
        _queries.AddRange(c.Queries);
        _resultSelectorFunctions.Add((resultIndex, c.ReadResultFrom));
        return this;
    }
}