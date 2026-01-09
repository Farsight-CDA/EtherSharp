using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace.Types;

public record TraceAction(
    CallType CallType,
    Address From,
    ulong Gas,
    byte[] Input,
    Address To,
    UInt256 Value
);
