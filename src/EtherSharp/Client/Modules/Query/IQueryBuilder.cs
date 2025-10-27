using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

public interface IQueryBuilder<TQuery> : ICallable<List<TQuery>>
{
    public IQueryBuilder<TQuery> AddQuery(ITxInput<TQuery> c);
    public IQueryBuilder<TQuery> AddSafeQuery(ITxInput<TQuery> c, Func<QueryResult<TQuery>, TQuery> mapping);

    public IQueryBuilder<TQuery> AddQuery<T1>(
        ITxInput<T1> c1,
        Func<T1, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1>(
        ITxInput<T1> c1,
        Func<QueryResult<T1>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2>(
        ITxInput<T1> c1, ITxInput<T2> c2,
        Func<T1, T2, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2>(
        ITxInput<T1> c1, ITxInput<T2> c2,
        Func<QueryResult<T1>, QueryResult<T2>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3,
        Func<T1, T2, T3, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4,
        Func<T1, T2, T3, T4, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5,
        Func<T1, T2, T3, T4, T5, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6,
        Func<T1, T2, T3, T4, T5, T6, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7,
        Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, QueryResult<T8>, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8, ITxInput<T9> c9,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8, ITxInput<T9> c9,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, QueryResult<T8>, QueryResult<T9>, TQuery> mapping
    );

    public Task<List<TQuery>> QueryAsync(TargetBlockNumber targetBlock = default, CancellationToken cancellationToken = default);
}
