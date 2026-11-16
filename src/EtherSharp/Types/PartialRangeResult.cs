namespace EtherSharp.Types;

/// <summary>
/// Represents the accepted successful prefix of a gas-bounded range of contract calls.
/// </summary>
/// <typeparam name="T">The decoded result type of each call.</typeparam>
/// <param name="Items">The accepted successful call results, in input order.</param>
public sealed record PartialRangeResult<T>(IReadOnlyList<T> Items);
