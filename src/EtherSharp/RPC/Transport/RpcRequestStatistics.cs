namespace EtherSharp.RPC.Transport;

/// <summary>
/// Collects outgoing JSON-RPC request attempts across participating transports.
/// </summary>
public class RpcRequestStatistics
{
    private long _rpcRequestCount;

    /// <summary>Gets the number of dispatched attempts, including failed attempts.</summary>
    public long RpcRequestCount => Interlocked.Read(ref _rpcRequestCount);

    /// <summary>
    /// Records one attempt immediately before transport dispatch. Custom transports must call this too.
    /// </summary>
    /// <param name="method">The JSON-RPC method being dispatched.</param>
    /// <remarks>Overrides may be called concurrently by participating transports.</remarks>
    public virtual void RecordRequest(string method)
        => Interlocked.Increment(ref _rpcRequestCount);
}
