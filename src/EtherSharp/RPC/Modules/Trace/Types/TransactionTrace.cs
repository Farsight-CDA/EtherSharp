namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// One execution entry in the transaction replay trace.
/// </summary>
/// <param name="Action">Input/action metadata for this trace step.</param>
/// <param name="Result">Output/result metadata for this trace step when available.</param>
/// <param name="Subtraces">Number of direct child trace entries.</param>
/// <param name="TraceAddress">Address path that identifies this step inside the call tree.</param>
/// <param name="Type">Node-reported trace type (for example <c>call</c> or <c>create</c>).</param>
/// <param name="Error">Node-reported error for this step, when execution failed.</param>
public record TransactionTrace(
    TraceAction Action,
    TraceResult? Result,
    int Subtraces,
    int[] TraceAddress,
    string Type,
    string? Error
);
