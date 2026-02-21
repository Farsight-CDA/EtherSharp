using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Input/action portion of a transaction trace entry.
/// </summary>
/// <param name="CallType">EVM call/create operation type.</param>
/// <param name="From">Caller address.</param>
/// <param name="Gas">Gas supplied to this execution step.</param>
/// <param name="Input">Call input bytes.</param>
/// <param name="To">Target address.</param>
/// <param name="Value">Native value sent with this step.</param>
public record TraceAction(
    CallType CallType,
    Address From,
    ulong Gas,
    byte[] Input,
    Address To,
    UInt256 Value
);
