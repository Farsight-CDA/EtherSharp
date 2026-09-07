using EtherSharp.Contract;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Defines hooks for observing interpreter execution.</summary>
public interface IInterpreterExecutionHooks
{
    /// <summary>Runs before instruction validation or operand mutation.</summary>
    /// <remarks>
    /// Execution remains paused until the callback completes. Readers are valid only during
    /// the callback. Do not re-enter or dispose the runtime from a callback.
    /// Exceptions abort the operation and restore its starting state.
    /// </remarks>
    ValueTask OnInstructionAsync(
        IInterpreterFrameReader frame,
        int programCounter,
        EvmOpcode opcode,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;
}
