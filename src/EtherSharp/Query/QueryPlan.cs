using EtherSharp.Contract;
using EtherSharp.Types;
using System.Runtime.InteropServices;

namespace EtherSharp.Query;

internal sealed class QueryPlan(
    int capacity = 0,
    IReadOnlyDictionary<Address, AccountOverride>? callerStateOverrides = null
) : IQueryPlan
{
    private readonly List<IQueryOperation> _queries = [with(capacity)];
    private readonly List<QueryOperationId> _requiredOperationIds = [];
    private readonly IReadOnlyDictionary<Address, AccountOverride>? _callerStateOverrides = callerStateOverrides;

    private Dictionary<Address, AccountOverride>? _stateOverrides;
    private Dictionary<Address, EVMRuntimeCode>? _flashCodes;

    public IReadOnlyList<IQueryOperation> Queries
        => _queries;

    public int Count
        => _queries.Count;

    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides
        => _stateOverrides ?? _callerStateOverrides;

    public IReadOnlyDictionary<Address, EVMRuntimeCode>? FlashCodes
        => _flashCodes;

    public bool RequiresQuerierStateOverride { get; private set; }

    public ReadOnlySpan<QueryOperationId> RequiredOperationIds
        => CollectionsMarshal.AsSpan(_requiredOperationIds);

    public void AddOperation(IQueryOperation query)
    {
        AddRequiredOperation(query.OperationId);
        _queries.Add(query);
    }

    public void Add(IQuery query)
        => query.AddTo(this);

    public void RequireQuerierStateOverride()
        => RequiresQuerierStateOverride = true;

    public int GetCallDataLength(QueryEncodingContext context)
    {
        int length = 0;
        foreach(var query in _queries)
        {
            length = checked(length + query.GetCallDataLength(context));
        }

        return length;
    }

    public void AddStateOverride(in Address address, AccountOverride accountOverride)
    {
        var stateOverrides = EnsureStateOverrides();

        ref var existingOverride = ref CollectionsMarshal.GetValueRefOrAddDefault(stateOverrides, address, out bool exists);
        if(exists)
        {
            if(existingOverride != accountOverride)
            {
                throw new InvalidOperationException($"Queries require conflicting state overrides for {address}.");
            }

            return;
        }

        existingOverride = accountOverride;
    }

    public void AddFlashCode(in Address stateOverrideAddress, EVMRuntimeCode runtimeCode)
        => (_flashCodes ??= []).TryAdd(stateOverrideAddress, runtimeCode);

    public void AddRequirements(
        IReadOnlyDictionary<Address, AccountOverride>? stateOverrides,
        IReadOnlyDictionary<Address, EVMRuntimeCode>? flashCodes,
        bool requiresQuerierStateOverride,
        ReadOnlySpan<QueryOperationId> requiredOperationIds
    )
    {
        if(stateOverrides is not null)
        {
            foreach(var (address, accountOverride) in stateOverrides)
            {
                AddStateOverride(address, accountOverride);
            }
        }

        if(flashCodes is not null)
        {
            foreach(var (stateOverrideAddress, runtimeCode) in flashCodes)
            {
                AddFlashCode(stateOverrideAddress, runtimeCode);
            }
        }

        if(requiresQuerierStateOverride)
        {
            RequireQuerierStateOverride();
        }

        foreach(var operationId in requiredOperationIds)
        {
            AddRequiredOperation(operationId);
        }
    }

    private void AddRequiredOperation(QueryOperationId operationId)
    {
        if(!_requiredOperationIds.Contains(operationId))
        {
            _requiredOperationIds.Add(operationId);
        }
    }

    private Dictionary<Address, AccountOverride> EnsureStateOverrides()
        => _stateOverrides ??= _callerStateOverrides is null
            ? []
            : [with(_callerStateOverrides)];
}
