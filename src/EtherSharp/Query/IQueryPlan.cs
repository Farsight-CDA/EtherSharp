using EtherSharp.Contract;
using EtherSharp.Types;

namespace EtherSharp.Query;

/// <summary>
/// Collects the low-level operations and state overrides required to execute a query.
/// </summary>
public interface IQueryPlan
{
    /// <summary>
    /// Adds a low-level operation to the plan.
    /// </summary>
    public void AddOperation(IQueryOperation operation);

    /// <summary>
    /// Adds state overrides, flash code, and Querier runtime requirements to this plan.
    /// </summary>
    public void AddRequirements(
        IReadOnlyDictionary<Address, AccountOverride>? stateOverrides,
        IReadOnlyDictionary<Address, EVMRuntimeCode>? flashCodes,
        bool requiresQuerierStateOverride,
        ReadOnlySpan<QueryOperationId> requiredOperationIds
    );

    /// <summary>
    /// Adds the operations and state overrides required by <paramref name="query"/>.
    /// </summary>
    public void Add(IQuery query);

    /// <summary>
    /// Requires the Querier runtime to also be installed at <see cref="IQuerier.StateOverride.Address"/>.
    /// </summary>
    public void RequireQuerierStateOverride();

    /// <summary>
    /// Adds a state override required by the query.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a different override has already been added for <paramref name="address"/>.
    /// </exception>
    public void AddStateOverride(in Address address, AccountOverride accountOverride);

    /// <summary>
    /// Requires runtime code called by flash call operations, identified by the account it is installed at with state overrides.
    /// </summary>
    /// <remarks>
    /// With state overrides the code is installed at <paramref name="stateOverrideAddress"/> before execution; otherwise the
    /// Querier deploys it before running any operation.
    /// </remarks>
    public void AddFlashCode(in Address stateOverrideAddress, EVMRuntimeCode runtimeCode);
}
