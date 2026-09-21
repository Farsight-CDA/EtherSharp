using EtherSharp.Interpreter.Runtime;

namespace EtherSharp.Interpreter.Forking;

/// <summary>Describes composable work in a structured fork run.</summary>
/// <typeparam name="TResult">The run result type.</typeparam>
public abstract class InterpreterRun<TResult> : InterpreterRun
{
    private InterpreterRun() { }

    internal async Task<TResult> ExecuteAsync(
        InterpreterStateFork fork,
        InterpreterStateFork.RunParticipant participant
    )
    {
        try
        {
            return await ExecuteCoreAsync(fork, participant);
        }
        finally
        {
            fork.CompleteParticipant(participant);
        }
    }

    private protected abstract ValueTask<TResult> ExecuteCoreAsync(
        InterpreterStateFork fork,
        InterpreterStateFork.RunParticipant participant
    );

    internal sealed class Lane(
        InterpreterRuntime interpreter,
        Func<IInterpreterLane, ValueTask<TResult>> execute,
        CancellationToken cancellationToken
    ) : InterpreterRun<TResult>
    {
        internal InterpreterRuntime Interpreter { get; } = interpreter;

        internal override InterpreterStateFork.RunParticipant.Lane CreateParticipant(
            InterpreterStateFork.RunState run,
            InterpreterStateFork.RunParticipant.Program? parent
        )
        {
            var participant = new InterpreterStateFork.RunParticipant.Lane(Interpreter)
            {
                Run = run,
                Parent = parent,
                CancellationToken = cancellationToken,
            };
            return participant;
        }

        private protected override ValueTask<TResult> ExecuteCoreAsync(
            InterpreterStateFork fork,
            InterpreterStateFork.RunParticipant participant
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            return execute(Interpreter);
        }
    }

    internal sealed class Program(
        Func<InterpreterRunContext, ValueTask<TResult>> execute
    ) : InterpreterRun<TResult>
    {
        internal override InterpreterStateFork.RunParticipant.Program CreateParticipant(
            InterpreterStateFork.RunState run,
            InterpreterStateFork.RunParticipant.Program? parent
        ) => new()
        {
            Run = run,
            Parent = parent,
        };

        private protected override ValueTask<TResult> ExecuteCoreAsync(
            InterpreterStateFork fork,
            InterpreterStateFork.RunParticipant participant
        ) => execute(new InterpreterRunContext(fork, (InterpreterStateFork.RunParticipant.Program) participant));
    }
}
