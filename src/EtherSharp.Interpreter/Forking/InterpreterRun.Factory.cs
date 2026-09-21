using EtherSharp.Interpreter.Runtime;

namespace EtherSharp.Interpreter.Forking;

/// <summary>Provides the base for structured interpreter runs and creates typed runs.</summary>
public abstract class InterpreterRun
{
    private protected InterpreterRun() { }

    internal abstract InterpreterStateFork.RunParticipant CreateParticipant(
        InterpreterStateFork.RunState run,
        InterpreterStateFork.RunParticipant.Program? parent
    );

    /// <summary>Creates a lane bound to an interpreter.</summary>
    /// <param name="interpreter">The interpreter to execute.</param>
    /// <param name="execute">The sequential work performed by the lane.</param>
    /// <param name="cancellationToken">Cancels before the lane starts or queues an uncached host request.</param>
    /// <remarks>
    /// Cancellation is cooperative at host-request boundaries. It does not interrupt opcode execution
    /// or requests already in flight, and a lane can complete if it needs no further uncached data.
    /// </remarks>
    public static InterpreterRun<TResult> For<TResult>(
        IInterpreter interpreter,
        Func<IInterpreterLane, ValueTask<TResult>> execute,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(interpreter);
        ArgumentNullException.ThrowIfNull(execute);
        return interpreter is InterpreterRuntime runtime
            ? new InterpreterRun<TResult>.Lane(runtime, execute, cancellationToken)
            : throw new ArgumentException("The interpreter was not created by an interpreter state fork.", nameof(interpreter));
    }

    /// <summary>Creates a composable run that can schedule interpreter work dynamically.</summary>
    public static InterpreterRun<TResult> Create<TResult>(
        Func<InterpreterRunContext, ValueTask<TResult>> execute
    )
    {
        ArgumentNullException.ThrowIfNull(execute);
        return new InterpreterRun<TResult>.Program(execute);
    }
}
