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
    public void AddOperation(IQuery operation);

    /// <summary>
    /// Adds the operations and state overrides required by <paramref name="query"/>.
    /// </summary>
    public void Add<T>(IQuery<T> query);

    /// <summary>
    /// Adds a state override required by the query.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a different override has already been added for <paramref name="address"/>.
    /// </exception>
    public void AddStateOverride(in Address address, AccountOverride accountOverride);
}
