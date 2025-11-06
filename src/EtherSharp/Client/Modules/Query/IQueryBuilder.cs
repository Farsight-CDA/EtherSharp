using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

public interface IQueryBuilder<TQuery> : IQuery<List<TQuery>>
{
    public IQueryBuilder<TQuery> AddQuery(IQuery<TQuery> c);
    public IQueryBuilder<TQuery> AddQuery<T1>(
        IQuery<T1> c1,
        Func<T1, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2,
        Func<T1, T2, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        Func<T1, T2, T3, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        Func<T1, T2, T3, T4, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        Func<T1, T2, T3, T4, T5, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6,
        Func<T1, T2, T3, T4, T5, T6, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7,
        Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8, IQuery<T9> c9,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping
    );

    public Task<List<TQuery>> QueryAsync(TargetBlockNumber targetBlock = default, CancellationToken cancellationToken = default);
}
