namespace EtherSharp.Types;

/// <summary>
/// Represents the fully completed prefix of a gas-bounded range of queries.
/// </summary>
/// <typeparam name="T">The result type of each query item.</typeparam>
/// <param name="Items">The results of fully completed query items, in input order.</param>
public sealed record PartialRangeResult<T>(IReadOnlyList<T> Items);
