using EtherSharp.Interpreter.Runtime;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Interpreter.Forking;

public sealed partial class InterpreterStateFork
{
    internal sealed class RunState
    {
        public List<RunParticipant> Participants { get; } = [];
        public bool IsFetching { get; set; }
    }

    internal abstract class RunParticipant
    {
        public required RunState Run { get; init; }
        public Program? Parent { get; init; }

        public virtual void Attach() { }
        public virtual void Detach() { }

        internal sealed class Lane(InterpreterRuntime interpreter) : RunParticipant
        {
            private TaskCompletionSource? _completion;

            public InterpreterRuntime Interpreter { get; } = interpreter;
            public List<HostRequest>? Requests { get; private set; }

            public override void Attach()
                => Interpreter.Participant = this;

            public override void Detach()
                => Interpreter.Participant = null;

            public TaskCompletionSource SetRequests(List<HostRequest> requests)
            {
                ArgumentNullException.ThrowIfNull(requests);
                if(requests.Count == 0)
                {
                    throw new ArgumentException("At least one upstream request is required.", nameof(requests));
                }
                if(Requests is not null || _completion is not null)
                {
                    throw new InvalidOperationException("An interpreter cannot have multiple concurrent upstream requests.");
                }

                Requests = requests;
                _completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                return _completion;
            }

            public void CompleteRequests()
            {
                var completion = _completion
                    ?? throw new InvalidOperationException("A pending request must have a completion source.");
                Requests = null;
                _completion = null;
                completion.TrySetResult();
            }

            public void FailRequests(Exception exception)
            {
                var completion = _completion
                    ?? throw new InvalidOperationException("A pending request must have a completion source.");
                Requests = null;
                _completion = null;
                completion.TrySetException(exception);
            }
        }

        internal sealed class Program : RunParticipant
        {
            public int RemainingChildren { get; set; }
        }
    }

    private RunState? _runState;

    internal ValueTask EnsureCachedAsync(
        InterpreterRuntime interpreter,
        List<HostRequest> requests
    )
    {
        TaskCompletionSource completion;
        HostRequest[]? batch = null;
        RunState run;
        lock(_lock)
        {
            var participant = interpreter.Participant
                ?? throw new InvalidOperationException("Interpreter operations must execute through a structured fork run.");
            run = participant.Run;

            ArgumentNullException.ThrowIfNull(requests);
            for(int i = requests.Count - 1; i >= 0; i--)
            {
                var request = requests[i];
                ArgumentNullException.ThrowIfNull(request);
                if(Cache.Contains(request))
                {
                    requests.RemoveAt(i);
                }
            }
            if(requests.Count == 0)
            {
                return ValueTask.CompletedTask;
            }

            completion = participant.SetRequests(requests);
            if(TakeBatchIfReady(out var readyBatch))
            {
                batch = readyBatch;
            }
        }

        if(batch is not null)
        {
            _ = ResolveAsync(run, batch);
        }
        return new ValueTask(completion.Task);
    }

    private bool TakeBatchIfReady(
        [MaybeNullWhen(false)] out HostRequest[] batch
    )
    {
        if(_runState is not { IsFetching: false } run)
        {
            batch = null;
            return false;
        }

        HashSet<HostRequest> requests = [];
        foreach(var participant in run.Participants)
        {
            switch(participant)
            {
                case RunParticipant.Program { RemainingChildren: > 0 }:
                    continue;
                case RunParticipant.Lane { Requests: { } pending }:
                    requests.UnionWith(pending);
                    break;
                default:
                    batch = null;
                    return false;
            }
        }

        if(requests.Count == 0)
        {
            batch = null;
            return false;
        }

        run.IsFetching = true;
        batch = [.. requests];
        return true;
    }

    private async Task ResolveAsync(RunState run, HostRequest[] batch)
    {
        HostRequest[]? nextBatch = null;
        try
        {
            var results = await _dataProvider.FetchAsync(Context, batch);

            lock(_lock)
            {
                foreach(var result in results)
                {
                    Cache.Store(result);
                }

                bool madeProgress = false;
                foreach(var participant in run.Participants)
                {
                    if(participant is not RunParticipant.Lane { Requests: { } requests } lane)
                    {
                        continue;
                    }

                    madeProgress |= requests.RemoveAll(Cache.Contains) != 0;
                    if(requests.Count == 0)
                    {
                        lane.CompleteRequests();
                    }
                }
                if(!madeProgress)
                {
                    throw new InvalidOperationException("The data provider did not resolve any pending value.");
                }

                // Unanswered requests remain pending. Retry immediately if every lane is still blocked;
                // otherwise a released interpreter will trigger the next batch when it blocks or completes.
                run.IsFetching = false;
                if(TakeBatchIfReady(out var readyBatch))
                {
                    nextBatch = readyBatch;
                }
            }
        }
        catch(Exception exception)
        {
            lock(_lock)
            {
                foreach(var participant in run.Participants)
                {
                    if(participant is RunParticipant.Lane { Requests: not null } lane)
                    {
                        lane.FailRequests(exception);
                    }
                }
                run.IsFetching = false;
            }
        }
        if(nextBatch is not null)
        {
            _ = ResolveAsync(run, nextBatch);
        }
    }

    /// <summary>Runs one composable run in a structured batching scope.</summary>
    public async Task<TResult> RunAsync<TResult>(InterpreterRun<TResult> run)
    {
        ArgumentNullException.ThrowIfNull(run);
        var program = run switch
        {
            InterpreterRun<TResult>.Program value => value,
            InterpreterRun<TResult>.Lane => new InterpreterRun<TResult>.Program(
                context => context.RunAsync(run)
            ),
            _ => throw new ArgumentException("Unsupported interpreter run type.", nameof(run)),
        };

        RunState runState;
        RunParticipant participant;
        lock(_lock)
        {
            if(_runState is not null)
            {
                throw new InvalidOperationException("Another interpreter run is already active on this fork.");
            }
            runState = new RunState();
            participant = program.CreateParticipant(runState, parent: null);
            runState.Participants.Add(participant);
            _runState = runState;
        }

        try
        {
            return await program.ExecuteAsync(this, participant);
        }
        finally
        {
            lock(_lock)
            {
                _runState = null;
            }
        }
    }

    /// <summary>Runs an arbitrary number of homogeneous composable runs concurrently.</summary>
    public Task<TResult[]> RunAsync<TResult>(
        params InterpreterRun<TResult>[] runs
    )
    {
        ArgumentNullException.ThrowIfNull(runs);
        return RunAsync(InterpreterRun.Create(context => context.RunAsync(runs)));
    }

    /// <summary>Runs two composable runs concurrently in a structured batching scope.</summary>
    public async Task<(T1 First, T2 Second)> RunAsync<T1, T2>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second
    ) => await RunAsync(
        InterpreterRun.Create(context => context.RunAsync(first, second))
    );

    /// <summary>Runs three composable runs concurrently in a structured batching scope.</summary>
    public async Task<(T1 First, T2 Second, T3 Third)> RunAsync<T1, T2, T3>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third
    ) => await RunAsync(
        InterpreterRun.Create(context => context.RunAsync(first, second, third))
    );

    /// <summary>Runs four composable runs concurrently in a structured batching scope.</summary>
    public async Task<(T1 First, T2 Second, T3 Third, T4 Fourth)> RunAsync<T1, T2, T3, T4>(
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third,
        InterpreterRun<T4> fourth
    ) => await RunAsync(
        InterpreterRun.Create(context => context.RunAsync(first, second, third, fourth))
    );

    internal async ValueTask<TResult> RunAsync<TResult>(
        RunParticipant.Program parent,
        InterpreterRun<TResult> run
    )
    {
        var participants = AddChildRuns(parent, run);
        return await run.ExecuteAsync(this, participants[0]);
    }

    internal async ValueTask<(T1 First, T2 Second)> RunAsync<T1, T2>(
        RunParticipant.Program parent,
        InterpreterRun<T1> first,
        InterpreterRun<T2> second
    )
    {
        var participants = AddChildRuns(parent, first, second);
        var firstTask = first.ExecuteAsync(this, participants[0]);
        var secondTask = second.ExecuteAsync(this, participants[1]);
        await Task.WhenAll(firstTask, secondTask);
        return (await firstTask, await secondTask);
    }

    internal async ValueTask<(T1 First, T2 Second, T3 Third)> RunAsync<T1, T2, T3>(
        RunParticipant.Program parent,
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third
    )
    {
        var participants = AddChildRuns(parent, first, second, third);
        var firstTask = first.ExecuteAsync(this, participants[0]);
        var secondTask = second.ExecuteAsync(this, participants[1]);
        var thirdTask = third.ExecuteAsync(this, participants[2]);
        await Task.WhenAll(firstTask, secondTask, thirdTask);
        return (await firstTask, await secondTask, await thirdTask);
    }

    internal async ValueTask<(T1 First, T2 Second, T3 Third, T4 Fourth)> RunAsync<T1, T2, T3, T4>(
        RunParticipant.Program parent,
        InterpreterRun<T1> first,
        InterpreterRun<T2> second,
        InterpreterRun<T3> third,
        InterpreterRun<T4> fourth
    )
    {
        var participants = AddChildRuns(parent, first, second, third, fourth);
        var firstTask = first.ExecuteAsync(this, participants[0]);
        var secondTask = second.ExecuteAsync(this, participants[1]);
        var thirdTask = third.ExecuteAsync(this, participants[2]);
        var fourthTask = fourth.ExecuteAsync(this, participants[3]);
        await Task.WhenAll(firstTask, secondTask, thirdTask, fourthTask);
        return (await firstTask, await secondTask, await thirdTask, await fourthTask);
    }

    internal async ValueTask<TResult[]> RunAsync<TResult>(
        RunParticipant.Program parent,
        InterpreterRun<TResult>[] runs
    )
    {
        ArgumentNullException.ThrowIfNull(runs);
        if(runs.Length == 0)
        {
            return [];
        }

        var participants = AddChildRuns(parent, runs);
        var tasks = new Task<TResult>[runs.Length];
        for(int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = runs[i].ExecuteAsync(this, participants[i]);
        }
        return await Task.WhenAll(tasks);
    }

    private RunParticipant[] AddChildRuns(
        RunParticipant.Program parent,
        params ReadOnlySpan<InterpreterRun> definitions
    )
    {
        lock(_lock)
        {
            if(_runState != parent.Run
                || !_runState.Participants.Contains(parent)
                || parent.RemainingChildren != 0)
            {
                throw new InvalidOperationException("The run context already has child work in progress or is no longer active.");
            }

            var participants = new RunParticipant[definitions.Length];
            HashSet<InterpreterRuntime> uniqueInterpreters = [];
            for(int i = 0; i < participants.Length; i++)
            {
                var definition = definitions[i]
                    ?? throw new ArgumentException("A run cannot be null.", nameof(definitions));
                switch(definition.CreateParticipant(parent.Run, parent))
                {
                    case RunParticipant.Lane lane when lane.Interpreter.Fork != this:
                        throw new ArgumentException("Every interpreter run must belong to this fork.");
                    case RunParticipant.Lane lane
                        when lane.Interpreter.Participant is not null
                            || !uniqueInterpreters.Add(lane.Interpreter):
                        throw new InvalidOperationException("An interpreter can occur only once in a structured run.");
                    case RunParticipant value:
                        participants[i] = value;
                        break;
                }
            }

            foreach(var participant in participants)
            {
                participant.Attach();
                parent.Run.Participants.Add(participant);
            }
            parent.RemainingChildren = participants.Length;
            return participants;
        }
    }

    internal void CompleteParticipant(RunParticipant participant)
    {
        HostRequest[]? batch = null;
        lock(_lock)
        {
            if(!participant.Run.Participants.Contains(participant))
            {
                throw new InvalidOperationException("A structured run participant completed while it had active children.");
            }
            if(participant is RunParticipant.Lane { Requests: not null }
                || participant is RunParticipant.Program { RemainingChildren: not 0 })
            {
                throw new InvalidOperationException("A waiting structured run participant cannot complete.");
            }

            bool removed = participant.Run.Participants.Remove(participant);
            Debug.Assert(removed);
            participant.Detach();

            if(participant.Parent is { } parent)
            {
                Debug.Assert(parent.RemainingChildren > 0);
                parent.RemainingChildren--;
            }
            if(TakeBatchIfReady(out var readyBatch))
            {
                batch = readyBatch;
            }
        }
        if(batch is not null)
        {
            _ = ResolveAsync(participant.Run, batch);
        }
    }
}
