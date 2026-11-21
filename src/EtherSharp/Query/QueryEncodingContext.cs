namespace EtherSharp.Query;

/// <summary>
/// Describes the Querier backend a query payload is encoded for.
/// </summary>
/// <remarks>Only valid while encoding a single payload, so it cannot be stored or captured.</remarks>
public readonly ref struct QueryEncodingContext
{
    /// <summary>
    /// Gets whether the Querier runs with state overrides.
    /// </summary>
    public bool UsesStateOverrides { get; }

    internal QueryEncodingContext(bool usesStateOverrides)
    {
        UsesStateOverrides = usesStateOverrides;
    }
}
