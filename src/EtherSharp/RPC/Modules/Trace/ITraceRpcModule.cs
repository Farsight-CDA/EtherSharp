using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace;

/// <summary>
/// Low-level wrapper for <c>trace_*</c> JSON-RPC methods.
/// </summary>
public interface ITraceRpcModule
{
    /// <summary>
    /// Executes a simulated call and returns the requested raw trace payloads.
    /// </summary>
    public Task<TransactionTraceResult> CallAsync(
        Address? from, Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        TraceTypes traceTypes, TargetHeight targetHeight = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Replays a transaction and returns requested trace payloads.
    /// </summary>
    /// <returns>
    /// The raw trace payload returned by the node, or <see langword="null"/> when the node returns a null result (for example, transaction not found).
    /// </returns>
    public Task<TransactionTraceResult?> ReplayTransactionAsync(
        Bytes32 txHash, TraceTypes traceTypes, CancellationToken cancellationToken = default
    );
}
