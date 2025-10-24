using EtherSharp.Tx;

namespace EtherSharp.Querier;

public interface IQueryBuilder<TQuery>
{
    public IQueryBuilder<TQuery> AddQuery(ITxInput<TQuery> call);
    public IQueryBuilder<TQuery> AddQueries(params IEnumerable<ITxInput<TQuery>> calls);

    public IQueryBuilder<TQuery> AddQuery<T1>(ITxInput<T1> call1, Func<T1, TQuery> mapping);
    public IQueryBuilder<TQuery> AddQuery<T1, T2>(ITxInput<T1> call1, ITxInput<T2> call2, Func<T1, T2, TQuery> mapping);
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(ITxInput<T1> call1, ITxInput<T2> call2, ITxInput<T3> call3, Func<T1, T2, T3, TQuery> mapping);

    public Task<TQuery[]> QueryAsync(CancellationToken cancellationToken = default);
}
