using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.ExecutionSpecs;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Schedules nested interpreter work within one structured fork run.
/// </summary>
/// <remarks>
/// Arbitrary awaits performed by a program or lane remain active batching participants. Such awaits
/// must not depend on state reads that the active participant prevents from being dispatched.
/// </remarks>
public sealed class InterpreterRunContext
{
    private readonly InterpreterStateFork _fork;
    private readonly InterpreterStateFork.RunParticipant.Program _participant;

    internal InterpreterRunContext(
        InterpreterStateFork fork,
        InterpreterStateFork.RunParticipant.Program participant
    )
    {
        _fork = fork;
        _participant = participant;
    }

    /// <summary>Creates an interpreter belonging to this context's fork.</summary>
    public IInterpreter CreateInterpreter(
        InterpreterExecutionSpec? executionSpec = null,
        InterpreterResourceLimits? resourceLimits = null
    ) => _fork.CreateInterpreter(executionSpec, resourceLimits);

    /// <summary>Clones an idle interpreter belonging to this context's fork.</summary>
    public IInterpreter CloneInterpreter(IInterpreter source)
        => _fork.CloneInterpreter(source);

    /// <summary>Runs one child run.</summary>
    public ValueTask<TResult> RunAsync<TResult>(InterpreterRun<TResult> run)
        => _fork.RunAsync(_participant, run);

    /// <summary>Runs two child runs concurrently.</summary>
    public ValueTask<(T1 First, T2 Second)> RunAsync<T1, T2>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second
    ) => _fork.RunAsync(_participant, first, second);

    /// <summary>Runs three child runs concurrently.</summary>
    public ValueTask<(T1 First, T2 Second, T3 Third)> RunAsync<T1, T2, T3>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third
    ) => _fork.RunAsync(_participant, first, second, third);

    /// <summary>Runs four child runs concurrently.</summary>
    public ValueTask<(T1 First, T2 Second, T3 Third, T4 Fourth)> RunAsync<T1, T2, T3, T4>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third,
        InterpreterRun<T4> fourth
    ) => _fork.RunAsync(_participant, first, second, third, fourth);

    /// <summary>Runs an arbitrary number of homogeneous child runs concurrently.</summary>
    public ValueTask<TResult[]> RunAsync<TResult>(
        params InterpreterRun<TResult>[] runs
    ) => _fork.RunAsync(_participant, runs);
}
