using EtherSharp.Client;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

/// <summary>
/// Provides debugging endpoints backed by the node debug RPC module.
/// </summary>
public interface IDebugModule
{
    /// <summary>
    /// Traces call execution for a mined transaction.
    /// </summary>
    /// <param name="transactionHash">Transaction hash to trace.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>
    /// A call trace rooted at the transaction entry call, or <see langword="null"/> when the transaction cannot be traced.
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(in Bytes32 transactionHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Traces call execution for a mined transaction.
    /// </summary>
    /// <param name="transactionHash">Hex-encoded transaction hash to trace.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>
    /// A call trace rooted at the transaction entry call, or <see langword="null"/> when the transaction cannot be traced.
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Traces a simulated call using Geth's call tracer.
    /// </summary>
    public Task<CallTrace> TraceCallCallsAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Captures the account state required to execute a simulated call.
    /// </summary>
    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Captures account state changed by a simulated call.
    /// </summary>
    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default
    );
}
