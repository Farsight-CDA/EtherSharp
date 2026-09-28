using EtherSharp.Query;

namespace EtherSharp.Interpreter.Forking;

/// <summary>Configures an interpreter fork independently of its upstream data provider.</summary>
public readonly record struct InterpreterForkOptions
{
    /// <summary>
    /// The execution gas budget for calls executed or simulated by this fork's interpreters.
    /// Defaults to the block context's gas limit when unspecified.
    /// </summary>
    /// <remarks>
    /// Does not change the block gas limit exposed by <c>GASLIMIT</c> or the gas limit of explicit transactions.
    /// </remarks>
    public ulong? CallGasLimit { get; init; }

    /// <summary>
    /// The query used by the pre/post-block client helpers to resolve named block targets.
    /// Defaults to EVM NUMBER. Numeric targets take precedence.
    /// </summary>
    public IQuery<ulong>? BlockHeightQuery { get; init; }

    /// <summary>
    /// Disables <c>BLOBBASEFEE</c> without probing it on the upstream RPC endpoint.
    /// </summary>
    /// <remarks>
    /// Use this for chains whose RPC handlers crash when opcode <c>0x4A</c> is executed.
    /// </remarks>
    public bool DisableBlobBaseFee { get; init; }

    /// <summary>
    /// Application-known upstream values matching the fork's state snapshot.
    /// For pre-block forks, these must describe the parent block's post-state.
    /// </summary>
    public IReadOnlyList<InterpreterDataResult>? InitialState { get; init; }
}
