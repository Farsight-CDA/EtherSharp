using EtherSharp.Query;

namespace EtherSharp.Client.Modules.Query;

/// <summary>
/// Incrementally composes multiple queries and projects each added query into a value of type <typeparamref name="TQuery"/>.
/// </summary>
/// <typeparam name="TQuery">The output type produced for each query entry added to the builder.</typeparam>
public partial class QueryBuilder<TQuery> : IQuery<List<TQuery>>
{
    private readonly List<IQuery> _queries = [];
    private readonly List<Func<ReadOnlySpan<ReadOnlyMemory<byte>>, TQuery>> _resultSelectorFunctions = [];

    /// <summary>
    /// Gets the flattened low-level operations that will be executed by the query executor.
    /// </summary>
    public IReadOnlyList<IQuery> Queries => _queries;
    IReadOnlyList<IQuery> IQuery<List<TQuery>>.Queries => _queries;

    List<TQuery> IQuery<List<TQuery>>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var results = new List<TQuery>(_resultSelectorFunctions.Count);
        foreach(var selectorFunc in _resultSelectorFunctions)
        {
            results.Add(selectorFunc(queryResults));
        }

        return results;
    }

    /// <summary>
    /// Adds a query that already yields <typeparamref name="TQuery"/>.
    /// </summary>
    /// <param name="c1">The query to append.</param>
    /// <returns>The current builder instance.</returns>
    public QueryBuilder<TQuery> AddQuery(IQuery<TQuery> c1)
    {
        int o1 = _queries.Count;
        _queries.AddRange(c1.Queries);
        _resultSelectorFunctions.Add(x => c1.ReadResultFrom(x[o1..]));
        return this;
    }

    /// <summary>
    /// Adds multiple queries that already yield <typeparamref name="TQuery"/>.
    /// </summary>
    /// <param name="queries">The queries to append in order.</param>
    /// <returns>The current builder instance.</returns>
    public QueryBuilder<TQuery> AddQueries(params ReadOnlySpan<IQuery<TQuery>> queries)
    {
        foreach(var query in queries)
        {
            AddQuery(query);
        }

        return this;
    }

    /// <summary>
    /// Adds multiple queries and maps each result to <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="T1">The source result type for each query.</typeparam>
    /// <param name="mapping">Maps each query result to <typeparamref name="TQuery"/>.</param>
    /// <param name="queries">The queries to append in order.</param>
    /// <returns>The current builder instance.</returns>
    public QueryBuilder<TQuery> AddQueries<T1>(Func<T1, TQuery> mapping, params ReadOnlySpan<IQuery<T1>> queries)
    {
        foreach(var query in queries)
        {
            AddQuery(query, mapping);
        }

        return this;
    }
}
