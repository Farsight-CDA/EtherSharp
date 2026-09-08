namespace EtherSharp.RPC.Transport;

/// <summary>
/// Configures transport-specific handling for an RPC request.
/// </summary>
public readonly record struct RpcRequestOptions
{
    /// <summary>
    /// Gets the caller-owned collector for outgoing RPC request attempts, or <see langword="null"/> to disable collection.
    /// Reuse a collector to aggregate requests, or supply separate collectors for independent operations.
    /// </summary>
    public RpcRequestStatistics? Statistics { get; init; }

    /// <summary>
    /// Gets the transport-specific routing key.
    /// </summary>
    public int TransportKey { get; init; }

    /// <summary>
    /// Gets the request priority.
    /// </summary>
    public int Priority { get; init; }
}
