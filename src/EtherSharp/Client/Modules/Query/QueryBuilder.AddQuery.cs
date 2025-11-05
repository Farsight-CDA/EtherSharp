
using System;
using System.Linq;
using System.Collections.Generic;

namespace EtherSharp.Client.Modules.Query;

internal partial class QueryBuilder<TQuery> : IQueryBuilder<TQuery>
{
    
    public IQueryBuilder<TQuery> AddQuery<T1>(IQueryable<T1> c1, Func<T1, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2>(IQueryable<T1> c1, IQueryable<T2> c2, Func<T1, T2, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, Func<T1, T2, T3, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, Func<T1, T2, T3, T4, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, Func<T1, T2, T3, T4, T5, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        int o5 = _calls.Count - resultIndex;
        _calls.AddRange(c5.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, Func<T1, T2, T3, T4, T5, T6, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        int o5 = _calls.Count - resultIndex;
        _calls.AddRange(c5.GetQueryInputs());
        int o6 = _calls.Count - resultIndex;
        _calls.AddRange(c6.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7, Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        int o5 = _calls.Count - resultIndex;
        _calls.AddRange(c5.GetQueryInputs());
        int o6 = _calls.Count - resultIndex;
        _calls.AddRange(c6.GetQueryInputs());
        int o7 = _calls.Count - resultIndex;
        _calls.AddRange(c7.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7, IQueryable<T8> c8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        int o5 = _calls.Count - resultIndex;
        _calls.AddRange(c5.GetQueryInputs());
        int o6 = _calls.Count - resultIndex;
        _calls.AddRange(c6.GetQueryInputs());
        int o7 = _calls.Count - resultIndex;
        _calls.AddRange(c7.GetQueryInputs());
        int o8 = _calls.Count - resultIndex;
        _calls.AddRange(c8.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]), c8.ReadResultFrom(x[o8..]))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7, IQueryable<T8> c8, IQueryable<T9> c9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        
        int o1 = _calls.Count - resultIndex;
        _calls.AddRange(c1.GetQueryInputs());
        int o2 = _calls.Count - resultIndex;
        _calls.AddRange(c2.GetQueryInputs());
        int o3 = _calls.Count - resultIndex;
        _calls.AddRange(c3.GetQueryInputs());
        int o4 = _calls.Count - resultIndex;
        _calls.AddRange(c4.GetQueryInputs());
        int o5 = _calls.Count - resultIndex;
        _calls.AddRange(c5.GetQueryInputs());
        int o6 = _calls.Count - resultIndex;
        _calls.AddRange(c6.GetQueryInputs());
        int o7 = _calls.Count - resultIndex;
        _calls.AddRange(c7.GetQueryInputs());
        int o8 = _calls.Count - resultIndex;
        _calls.AddRange(c8.GetQueryInputs());
        int o9 = _calls.Count - resultIndex;
        _calls.AddRange(c9.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, x => mapping(c1.ReadResultFrom(x[o1..]), c2.ReadResultFrom(x[o2..]), c3.ReadResultFrom(x[o3..]), c4.ReadResultFrom(x[o4..]), c5.ReadResultFrom(x[o5..]), c6.ReadResultFrom(x[o6..]), c7.ReadResultFrom(x[o7..]), c8.ReadResultFrom(x[o8..]), c9.ReadResultFrom(x[o9..]))));
        return this;
    }
}
