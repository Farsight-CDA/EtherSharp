namespace EtherSharp.RPC.Transport;

/// <summary>
/// Configures transport-specific handling for an RPC request.
/// </summary>
public readonly record struct RpcRequestOptions
{
    /// <summary>
    /// Gets the transport-specific routing key.
    /// </summary>
    public int TransportKey { get; init; }

    /// <summary>
    /// Gets the request priority.
    /// </summary>
    public int Priority { get; init; }
}
