using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Precompiles;

/// <summary>
/// Represents the message-call context supplied to a precompile.
/// </summary>
/// <param name="Context">The block and transaction execution context.</param>
/// <param name="Origin">The transaction origin.</param>
/// <param name="Caller">The immediate message caller.</param>
/// <param name="To">The account whose storage and address context are used.</param>
/// <param name="Value">The native value exposed through <c>msg.value</c>.</param>
/// <param name="Input">The complete message-call input.</param>
/// <param name="Depth">The zero-based message-call depth.</param>
/// <param name="IsStatic">Whether state changes are prohibited.</param>
public readonly record struct PrecompileCall(
    InterpreterContext Context,
    Address Origin,
    Address Caller,
    Address To,
    UInt256 Value,
    ReadOnlyMemory<byte> Input,
    int Depth,
    bool IsStatic
);
