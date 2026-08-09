namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents one unchanged, added, removed, or modified state value.
/// </summary>
public abstract record StateChange<T>
{
    /// <summary>
    /// The value did not change.
    /// </summary>
    public sealed record Same : StateChange<T>;

    /// <summary>
    /// The value was added.
    /// </summary>
    /// <param name="Value">Added value.</param>
    public sealed record Added(T Value) : StateChange<T>;

    /// <summary>
    /// The value was removed.
    /// </summary>
    /// <param name="Value">Removed value.</param>
    public sealed record Removed(T Value) : StateChange<T>;

    /// <summary>
    /// The value changed from one value to another.
    /// </summary>
    /// <param name="From">Value before execution.</param>
    /// <param name="To">Value after execution.</param>
    public sealed record Changed(T From, T To) : StateChange<T>;
}
