using EtherSharp.Client;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Debug;

/// <summary>
/// Low-level wrapper for <c>debug_*</c> JSON-RPC methods.
/// </summary>
public interface IDebugRpcModule
{
    /// <summary>
    /// Traces calls executed by a mined transaction.
    /// </summary>
    /// <returns>
    /// The traced call tree for the transaction, or <see langword="null"/> when the node returns a null result (for example, transaction not found).
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(in Bytes32 transactionHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Traces calls executed by a mined transaction.
    /// </summary>
    /// <returns>
    /// The traced call tree for the transaction, or <see langword="null"/> when the node returns a null result (for example, transaction not found).
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Traces a simulated call using Geth's call tracer.
    /// </summary>
    public Task<CallTrace> TraceCallCallsAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures the account state required to execute a simulated call.
    /// </summary>
    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures account state changed by a simulated call.
    /// </summary>
    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default);
}
