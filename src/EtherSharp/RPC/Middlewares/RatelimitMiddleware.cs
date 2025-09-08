namespace EtherSharp.RPC.Middlewares;
public class RatelimitMiddleware : IRpcMiddleware, IDisposable
{
    private readonly TimeSpan _windowSize;
    private readonly int _requestsPerWindow;

    private readonly SemaphoreSlim _requestSemaphore;
    private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

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

    public void Dispose()
    {
        _disposeCts.Cancel();
        GC.SuppressFinalize(this);
    }

    public async Task<RpcResult<TResult>> HandleAsync<TResult>(Func<CancellationToken, Task<RpcResult<TResult>>> onNext, CancellationToken cancellationToken)
    {
        await _requestSemaphore.WaitAsync(cancellationToken);
        return await onNext(cancellationToken);
    }
}
