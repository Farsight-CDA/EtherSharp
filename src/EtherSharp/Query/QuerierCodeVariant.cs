namespace EtherSharp.Query;

/// <summary>
/// Identifies which backends a Querier operation handler region is built for.
/// </summary>
internal enum QuerierCodeVariant : byte
{
    /// <summary>Used by every backend.</summary>
    Shared = 0,
    /// <summary>Used when the node supports state overrides.</summary>
    StateOverride = 1,
    /// <summary>Used when the node does not support state overrides.</summary>
    Deploying = 2
}
