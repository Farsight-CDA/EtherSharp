using EtherSharp.Contract;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Defines hooks for observing interpreter execution.</summary>
/// <remarks>
/// Callbacks pause execution; readers and borrowed result bytes are valid only during the callback.
/// Context and transaction data are borrowed and must not be modified. Do not re-enter or dispose the runtime.
/// Exceptions restore the starting state and skip remaining exit/end callbacks.
/// </remarks>
public interface IInterpreterExecutionHooks
{
    /// <summary>Runs once after overrides and environment setup, before outer execution validation or mutation.</summary>
    /// <remarks>Includes outer bytecode, creation, and precompile invocations.</remarks>
    /// <param name="context">The block environment used for execution.</param>
    /// <param name="transaction">The transaction environment, including for simulated calls.</param>
    /// <param name="state">The interpreter state with simulation overrides applied.</param>
    ValueTask OnExecutionStartAsync(
        InterpreterContext context,
        TransactionEnvironment transaction,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs before an invocation's entry checks or SELFDESTRUCT transfer.</summary>
    /// <remarks>Includes the outer invocation, precompiles, and rejected entries.</remarks>
    ValueTask OnCallEnterAsync(IInterpreterFrameReader frame, IInterpreterStateReader state)
        => ValueTask.CompletedTask;

    /// <summary>Runs before instruction validation or operand mutation.</summary>
    ValueTask OnInstructionAsync(
        IInterpreterFrameReader frame,
        int programCounter,
        EvmOpcode opcode,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs after an invocation finalizes, including rollback and creation-code validation.</summary>
    /// <remarks>Shares the frame instance with entry and instruction callbacks.</remarks>
    ValueTask OnCallExitAsync(
        IInterpreterFrameReader frame,
        ExecutionResult result,
        IInterpreterStateReader state
    ) => ValueTask.CompletedTask;

    /// <summary>Runs once after the outer execution has finalized, before its state is committed or discarded.</summary>
    /// <remarks>
    /// Includes precompiles and EVM entry failures. State reflects EVM rollback, before simulation cleanup.
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
