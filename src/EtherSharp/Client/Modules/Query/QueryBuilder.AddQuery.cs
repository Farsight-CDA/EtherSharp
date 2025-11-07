namespace EtherSharp.Client.Modules.Query;

public partial class QueryBuilder<TQuery>
{
    public QueryBuilder<TQuery> AddQuery<T1>(IQuery<T1> c1, Func<T1, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2>(IQuery<T1> c1, IQuery<T2> c2, Func<T1, T2, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, Func<T1, T2, T3, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, Func<T1, T2, T3, T4, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, Func<T1, T2, T3, T4, T5, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        int o5 = _queries.Count - resultIndex;
        _queries.AddRange(c5.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, Func<T1, T2, T3, T4, T5, T6, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        int o5 = _queries.Count - resultIndex;
        _queries.AddRange(c5.Queries);
        int o6 = _queries.Count - resultIndex;
        _queries.AddRange(c6.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        int o5 = _queries.Count - resultIndex;
        _queries.AddRange(c5.Queries);
        int o6 = _queries.Count - resultIndex;
        _queries.AddRange(c6.Queries);
        int o7 = _queries.Count - resultIndex;
        _queries.AddRange(c7.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        int o5 = _queries.Count - resultIndex;
        _queries.AddRange(c5.Queries);
        int o6 = _queries.Count - resultIndex;
        _queries.AddRange(c6.Queries);
        int o7 = _queries.Count - resultIndex;
        _queries.AddRange(c7.Queries);
        int o8 = _queries.Count - resultIndex;
        _queries.AddRange(c8.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]), c8.ReadResultFrom(x[o8..]))));
        return this;
    }

    public QueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8, IQuery<T9> c9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping)
    {
        int resultIndex = _queries.Count;

        int o1 = _queries.Count - resultIndex;
        _queries.AddRange(c1.Queries);
        int o2 = _queries.Count - resultIndex;
        _queries.AddRange(c2.Queries);
        int o3 = _queries.Count - resultIndex;
        _queries.AddRange(c3.Queries);
        int o4 = _queries.Count - resultIndex;
        _queries.AddRange(c4.Queries);
        int o5 = _queries.Count - resultIndex;
        _queries.AddRange(c5.Queries);
        int o6 = _queries.Count - resultIndex;
        _queries.AddRange(c6.Queries);
        int o7 = _queries.Count - resultIndex;
        _queries.AddRange(c7.Queries);
        int o8 = _queries.Count - resultIndex;
        _queries.AddRange(c8.Queries);
        int o9 = _queries.Count - resultIndex;
        _queries.AddRange(c9.Queries);
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]), c8.ReadResultFrom(x[o8..]), c9.ReadResultFrom(x[o9..]))));
        return this;
    }
}
