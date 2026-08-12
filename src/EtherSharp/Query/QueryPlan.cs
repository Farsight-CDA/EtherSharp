using EtherSharp.Types;

namespace EtherSharp.Query;

internal sealed class QueryPlan(
    int capacity = 0,
    IReadOnlyDictionary<Address, AccountOverride>? callerStateOverrides = null
) : IQueryPlan
{
    private readonly List<IQuery> _queries = new List<IQuery>(capacity);
    private readonly IReadOnlyDictionary<Address, AccountOverride>? _callerStateOverrides = callerStateOverrides;

    private Dictionary<Address, AccountOverride>? _stateOverrides;

    public IReadOnlyList<IQuery> Queries
        => _queries;

    public int Count
        => _queries.Count;

    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides
        => _stateOverrides ?? _callerStateOverrides;

    public void AddOperation(IQuery query)
        => _queries.Add(query);

    public void Add<T>(IQuery<T> query)
        => query.AddTo(this);

    public void AddStateOverride(in Address address, AccountOverride accountOverride)
    {
        var stateOverrides = EnsureStateOverrides();

        if(stateOverrides.TryGetValue(address, out var existingOverride))
        {
            if(existingOverride != accountOverride)
            {
                throw new InvalidOperationException($"Queries require conflicting state overrides for {address}.");
            }

            return;
        }

        stateOverrides.Add(address, accountOverride);
    }

    private Dictionary<Address, AccountOverride> EnsureStateOverrides()
        => _stateOverrides ??= _callerStateOverrides is null
            ? []
            : new Dictionary<Address, AccountOverride>(_callerStateOverrides);
}
