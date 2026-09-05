using EtherSharp.Interpreter.Runtime;
using System.Diagnostics;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Creates independent interpreters over one cached upstream state and block context.
/// </summary>
/// <param name="dataProvider">The provider used to fetch upstream state.</param>
/// <param name="context">The block context shared by every interpreter created from this fork.</param>
public sealed partial class InterpreterStateFork(
    IInterpreterDataProvider dataProvider,
    InterpreterContext context
)
{
    private sealed class PendingRequest(InterpreterDataRequest request)
    {
        public InterpreterDataRequest Request { get; } = request;
        public TaskCompletionSource Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int WaiterCount { get; set; }
    }

    private readonly IInterpreterDataProvider _dataProvider = dataProvider
        ?? throw new ArgumentNullException(nameof(dataProvider));

    private readonly Lock _lock = new();
    private readonly Dictionary<InterpreterDataRequest, PendingRequest> _pending = [];
    private readonly InterpreterStateCache _cache = new();
    private int _participantCount;
    private int _waitingCount;
    private bool _isFetching;

    /// <summary>
    /// Gets the block context shared by interpreters created from this fork.
    /// </summary>
    public InterpreterContext Context { get; } = context
        ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Creates and registers an independent interpreter over this state fork.
    /// </summary>
    /// <param name="executionSpec">The consensus behavior used by the interpreter.</param>
    /// <param name="options">The interpreter resource limits.</param>
    /// <returns>An interpreter that must be disposed when it no longer participates in batching.</returns>
    /// <remarks>
    /// Separate interpreters created from this fork may execute concurrently, but each interpreter must
    /// be used sequentially and must not be disposed during an operation. A live interpreter that is
    /// neither executing nor waiting for state prevents the fork from flushing.
    /// </remarks>
    public InterpreterRuntime CreateInterpreter(
        InterpreterExecutionSpec executionSpec,
        InterpreterOptions? options = null
    )
    {
        ArgumentNullException.ThrowIfNull(executionSpec);
        var session = new InterpreterSession(this);
        var runtime = new InterpreterRuntime(
            Context,
            session,
            executionSpec,
            options?.Validate() ?? InterpreterOptions.Default,
            executionSpec.ValidateAndCreatePrecompileLookup()
        );

        lock(_lock)
        {
            _participantCount++;
        }

        return runtime;
    }

    private ValueTask<TValue> GetAsync<TKey, TValue>(
        InterpreterSession session,
        Dictionary<TKey, TValue> cache,
        TKey key,
        Func<TKey, InterpreterDataRequest> createRequest
    ) where TKey : notnull
    {
        PendingRequest pending;
        PendingRequest[]? batch;
        lock(_lock)
        {
            Debug.Assert(!session.IsUnregistered, "An unregistered session cannot read upstream state.");
            Debug.Assert(!session.IsReadInProgress, "An interpreter cannot have multiple concurrent upstream requests.");

            if(cache.TryGetValue(key, out var value))
            {
                return new ValueTask<TValue>(value);
            }

            var request = createRequest(key);
            if(!_pending.TryGetValue(request, out pending!))
            {
                pending = new PendingRequest(request);
                _pending.Add(request, pending);
            }

            session.IsReadInProgress = true;
            _waitingCount++;
            pending.WaiterCount++;
            batch = TakeBatchIfReady();
        }

        if(batch is not null)
        {
            _ = ResolveAsync(batch);
        }

        return AwaitValueAsync(session, pending.Completion.Task, cache, key);
    }

    private async ValueTask<TValue> AwaitValueAsync<TKey, TValue>(
        InterpreterSession session,
        Task completion,
        Dictionary<TKey, TValue> cache,
        TKey key
    ) where TKey : notnull
    {
        try
        {
            await completion;
            lock(_lock)
            {
                return cache[key];
            }
        }
        finally
        {
            lock(_lock)
            {
                session.IsReadInProgress = false;
            }
        }
    }

    private void RemoveInterpreter(InterpreterSession session)
    {
        PendingRequest[]? batch;
        lock(_lock)
        {
            if(session.IsUnregistered)
            {
                return;
            }

            Debug.Assert(!session.IsReadInProgress, "An interpreter cannot be unregistered during an operation.");
            session.IsUnregistered = true;
            _participantCount--;
            batch = TakeBatchIfReady();
        }

        if(batch is not null)
        {
            _ = ResolveAsync(batch);
        }
    }

    private PendingRequest[]? TakeBatchIfReady()
    {
        if(_isFetching || _participantCount == 0 || _waitingCount != _participantCount || _pending.Count == 0)
        {
            return null;
        }

        PendingRequest[] requests = [.. _pending.Values];
        _isFetching = true;
        return requests;
    }

    private async Task ResolveAsync(PendingRequest[] batch)
    {
        try
        {
            var requests = new InterpreterDataRequest[batch.Length];
            for(int i = 0; i < batch.Length; i++)
            {
                requests[i] = batch[i].Request;
            }
            var results = await _dataProvider.FetchAsync(Context, requests);

            lock(_lock)
            {
                foreach(var result in results)
                {
                    _cache.Store(result);
                }

                bool madeProgress = false;
                foreach(var pending in _pending.Values)
                {
                    if(_cache.Contains(pending.Request))
                    {
                        _pending.Remove(pending.Request);
                        _waitingCount -= pending.WaiterCount;
                        pending.Completion.TrySetResult();
                        madeProgress = true;
                    }
                }
                if(!madeProgress)
                {
                    throw new InvalidOperationException("The data provider did not resolve any pending value.");
                }

                // Unanswered requests remain pending for the next batch. Released interpreters must run again
                _isFetching = false;
            }
        }
        catch(Exception exception)
        {
            lock(_lock)
            {
                foreach(var pending in batch)
                {
                    if(!pending.Completion.Task.IsCompleted)
                    {
                        _pending.Remove(pending.Request);
                        _waitingCount -= pending.WaiterCount;
                        pending.Completion.TrySetException(exception);
                    }
                }
                _isFetching = false;
            }
        }
    }
}
