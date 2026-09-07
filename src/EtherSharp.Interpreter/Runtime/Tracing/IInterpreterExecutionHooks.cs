using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

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

    /// <summary>Runs before an invocation's entry checks.</summary>
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

    /// <summary>Runs after SELFDESTRUCT applies its balance changes and schedules deletion when applicable.</summary>
    /// <remarks>The effects may still be rolled back by an enclosing invocation.</remarks>
    /// <param name="frame">The active invocation executing SELFDESTRUCT.</param>
    /// <param name="beneficiary">The beneficiary specified by the instruction.</param>
    /// <param name="balance">The contract balance before SELFDESTRUCT applied its changes.</param>
    /// <param name="state">The state after the instruction's changes.</param>
    ValueTask OnSelfDestructAsync(
        IInterpreterFrameReader frame,
        Address beneficiary,
        UInt256 balance,
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
