namespace EtherSharp.RPC.Middlewares;

/// <summary>
/// Limits the number of RPC requests that can execute within a configured time window.
/// </summary>
public class RatelimitMiddleware : IRpcMiddleware, IDisposable
{
    private readonly TimeSpan _windowSize;
    private readonly int _requestsPerWindow;

    private readonly SemaphoreSlim _requestSemaphore;
    private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

    /// <summary>
    /// Initializes a new instance of the <see cref="RatelimitMiddleware"/> class.
    /// </summary>
    /// <param name="windowSize">The duration of each rate-limit window.</param>
    /// <param name="requestsPerWindow">The maximum number of requests allowed per window.</param>
    public RatelimitMiddleware(TimeSpan windowSize, int requestsPerWindow)
    {
        _windowSize = windowSize;
        _requestsPerWindow = requestsPerWindow;
        _requestSemaphore = new SemaphoreSlim(requestsPerWindow, requestsPerWindow);

        _ = Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(_windowSize);

            while(await timer.WaitForNextTickAsync(_disposeCts.Token))
            {
                int releaseCount = _requestsPerWindow - _requestSemaphore.CurrentCount;
                if(releaseCount > 0)
                {
                    _requestSemaphore.Release(releaseCount);
                }
            }
        });
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposeCts.Cancel();
        GC.SuppressFinalize(this);
    }

    async Task<RpcResult<TResult>> IRpcMiddleware.HandleAsync<TResult>(Func<CancellationToken, Task<RpcResult<TResult>>> onNext, CancellationToken cancellationToken)
    {
        await _requestSemaphore.WaitAsync(cancellationToken);
        return await onNext(cancellationToken);
    }
}
