namespace EtherSharp.Query;

/// <summary>
/// Represents the outcome of a query-backed contract call.
/// </summary>
/// <typeparam name="T">The decoded success payload type.</typeparam>
public abstract record QueryResult<T>
{
    /// <summary>
    /// Represents a successful call result.
    /// </summary>
    /// <param name="Value">The decoded return value.</param>
    public record Success(T Value) : QueryResult<T>;

    /// <summary>
    /// Represents a reverted call result.
    /// </summary>
    /// <param name="Data">The raw EVM revert data.</param>
    public record Reverted(ReadOnlyMemory<byte> Data) : QueryResult<T>;
}
