namespace EtherSharp.Client;

/// <summary>
/// Immutable configuration describing how an Ether client instance should behave.
/// </summary>
public sealed record EtherClientOptions
{
    /// <summary>
    /// Gets whether the client was built with transaction capabilities enabled.
    /// </summary>
    public bool IsTxClient { get; init; }
}
