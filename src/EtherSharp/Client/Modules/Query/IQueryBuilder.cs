using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

public interface IQueryBuilder<TQuery> : ICallable<List<TQuery>>
{
    public IQueryBuilder<TQuery> AddQuery(ICallable<TQuery> callable);
    public IQueryBuilder<TQuery> AddQueries(params IEnumerable<ICallable<TQuery>> callables);

    public IQueryBuilder<TQuery> AddQuery<T1>(ICallable<T1> c1, Func<T1, TQuery> mapping);
    public IQueryBuilder<TQuery> AddQuery<T1, T2>(ITxInput<T1> c1, ITxInput<T2> c2, Func<T1, T2, TQuery> mapping);
    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, Func<T1, T2, T3, TQuery> mapping);

    public Task<List<TQuery>> QueryAsync(TargetBlockNumber targetBlock = default, CancellationToken cancellationToken = default);
}
