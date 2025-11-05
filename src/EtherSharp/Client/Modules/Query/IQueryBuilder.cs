using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

public interface IQueryBuilder<TQuery> : IQueryable<List<TQuery>>
{
    public IQueryBuilder<TQuery> AddQuery(IQueryable<TQuery> c);
    public IQueryBuilder<TQuery> AddQuery<T1>(
        IQueryable<T1> c1,
        Func<T1, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2>(
        IQueryable<T1> c1, IQueryable<T2> c2,
        Func<T1, T2, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3,
        Func<T1, T2, T3, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4,
        Func<T1, T2, T3, T4, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5,
        Func<T1, T2, T3, T4, T5, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6,
        Func<T1, T2, T3, T4, T5, T6, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7,
        Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7, IQueryable<T8> c8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping
    );
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IQueryable<T1> c1, IQueryable<T2> c2, IQueryable<T3> c3, IQueryable<T4> c4, IQueryable<T5> c5, IQueryable<T6> c6, IQueryable<T7> c7, IQueryable<T8> c8, IQueryable<T9> c9,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping
    );

    public Task<List<TQuery>> QueryAsync(TargetBlockNumber targetBlock = default, CancellationToken cancellationToken = default);
}
