using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Provides read-only access to a call invocation, including calls that do not execute bytecode.</summary>
public interface IInterpreterFrameReader
{
    /// <summary>Gets the invocation ID, assigned sequentially within an execution with zero identifying the outer invocation.</summary>
    int Id { get; }
    /// <summary>Gets the parent invocation, or <see langword="null"/> for the outer invocation.</summary>
    IInterpreterFrameReader? Parent { get; }
    /// <summary>Gets the CALL, CALLCODE, DELEGATECALL, STATICCALL, CREATE, or CREATE2 operation.</summary>
    EvmOpcode Type { get; }
    /// <summary>Gets the account initiating this invocation.</summary>
    Address From { get; }
    /// <summary>Gets the invoked code address or created address.</summary>
    Address To { get; }
    /// <summary>Gets the execution address, which can differ from the invoked code address.</summary>
    Address Address { get; }
    /// <summary>Gets the caller exposed to executing code, including inherited DELEGATECALL context.</summary>
    Address Caller { get; }
    /// <summary>Gets the call value or creation endowment.</summary>
    UInt256 Value { get; }
    /// <summary>Gets borrowed calldata or creation initcode.</summary>
    ReadOnlyMemory<byte> Input { get; }
    /// <summary>Gets the invocation depth; the outer invocation has depth zero.</summary>
    int Depth { get; }
    /// <summary>Gets whether execution is in a static context.</summary>
    bool IsStatic { get; }
}
