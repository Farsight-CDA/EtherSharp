namespace EtherSharp.RPC.Transport;

/// <summary>Provides custom request option keys recognized by EtherSharp transports.</summary>
public static class RpcRequestOptionsKeys
{
    /// <summary>Gets the request statistics collector key recognized by built-in transports.</summary>
    /// <remarks>
    /// Set this key to a <see cref="RpcRequestStatistics"/> instance to collect dispatched request attempts.
    /// </remarks>
    public static RpcRequestOptionsKey<RpcRequestStatistics> Statistics { get; }
        = new("EtherSharp.RPC.Transport.Statistics");
}
