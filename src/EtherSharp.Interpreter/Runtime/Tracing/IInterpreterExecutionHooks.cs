using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Defines hooks for observing interpreter execution.</summary>
/// <remarks>
/// Execution remains paused until each callback completes. Readers and borrowed result bytes
/// are valid only during the callback. Do not re-enter or dispose the runtime from a callback.
/// Exceptions abort the operation and restore its starting state; no synthetic exit is emitted.
/// </remarks>
public interface IInterpreterExecutionHooks
{
    /// <summary>Runs after entry checks pass, before a bytecode frame begins execution.</summary>
    ValueTask OnFrameEnterAsync(IInterpreterFrameReader frame, IInterpreterStateReader state)
        => ValueTask.CompletedTask;

    /// <summary>Runs before instruction validation or operand mutation.</summary>
    ValueTask OnInstructionAsync(
        IInterpreterFrameReader frame,
        int programCounter,
        EvmOpcode opcode,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs after frame finalization, including rollback and creation-code validation.</summary>
    ValueTask OnFrameExitAsync(
        IInterpreterFrameReader frame,
        ExecutionResult result,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs after a precompile invocation completes and any call-level rollback is applied.</summary>
    /// <param name="precompileAddress">The code address identifying the precompile, which may differ from the execution address.</param>
    /// <param name="call">The precompile invocation context.</param>
    /// <param name="result">The invocation outcome.</param>
    /// <param name="state">The current interpreter state.</param>
    ValueTask OnPrecompileCallAsync(
        Address precompileAddress,
        PrecompileCall call,
        ExecutionResult result,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs once after the outer execution has finalized, before its state is committed or discarded.</summary>
    /// <remarks>
    /// Includes precompile calls and EVM entry failures. Exceptions that abort execution do not emit this callback.
    /// State reflects the execution outcome, including EVM rollback, before simulation cleanup.
    /// Context and transaction data are borrowed and must not be modified.
    /// </remarks>
    /// <param name="context">The block environment used for execution.</param>
    /// <param name="transaction">The transaction environment, including for simulated calls.</param>
    /// <param name="result">The finalized outer execution outcome.</param>
    /// <param name="state">The current interpreter state.</param>
    ValueTask OnExecutionEndAsync(
        InterpreterContext context,
        TransactionEnvironment transaction,
        ExecutionResult result,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;
}
