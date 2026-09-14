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
    public static InterpreterRun<TResult> For<TResult>(
        IInterpreter interpreter,
        Func<IInterpreterLane, ValueTask<TResult>> execute
    )
    {
        ArgumentNullException.ThrowIfNull(interpreter);
        ArgumentNullException.ThrowIfNull(execute);
        return interpreter is InterpreterRuntime runtime
            ? new InterpreterRun<TResult>.Lane(runtime, execute)
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
