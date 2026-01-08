using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record CallTrace(
    Address From,
    Address To,
    ulong Gas,
    ulong GasUsed,
    UInt256 Value,
    byte[] Input,
    byte[] Output,
    CallType Type,
    CallTrace[]? Calls
);
