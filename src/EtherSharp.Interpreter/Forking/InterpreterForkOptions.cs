using EtherSharp.Query;

namespace EtherSharp.Interpreter.Forking;

/// <summary>Configures an interpreter fork independently of its upstream data provider.</summary>
public readonly record struct InterpreterForkOptions
{
    /// <summary>
    /// The query used by the pre/post-block client helpers to resolve named block targets.
    /// Defaults to EVM NUMBER. Numeric targets take precedence.
    /// </summary>
    public IQuery<ulong>? BlockHeightQuery { get; init; }

    /// <summary>
    /// Application-known upstream values matching the fork's state snapshot.
    /// For pre-block forks, these must describe the parent block's post-state.
    /// </summary>
    public IReadOnlyList<InterpreterDataResult>? InitialState { get; init; }
}
