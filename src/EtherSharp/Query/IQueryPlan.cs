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
    /// Adds state overrides and a Querier runtime requirement to this plan.
    /// </summary>
    public void AddRequirements(
        IReadOnlyDictionary<Address, AccountOverride>? stateOverrides,
        bool requiresQuerierStateOverride
    );

    /// <summary>
    /// Adds the operations and state overrides required by <paramref name="query"/>.
    /// </summary>
    public void Add(IQuery query);

    /// <summary>
    /// Requires the selected Querier runtime to be installed at its fixed state override address.
    /// </summary>
    public void RequireQuerierStateOverride();

    /// <summary>
    /// Adds a state override required by the query.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a different override has already been added for <paramref name="address"/>.
    /// </exception>
    public void AddStateOverride(in Address address, AccountOverride accountOverride);
}
