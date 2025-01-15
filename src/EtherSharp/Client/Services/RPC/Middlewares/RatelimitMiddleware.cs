namespace EtherSharp.Client.Services.RPC.Middlewares;
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

            while (await timer.WaitForNextTickAsync(_disposeCts.Token))
            {
                _requestSemaphore.Release(_requestsPerWindow - _requestSemaphore.CurrentCount);
            }
        });
    }

    public void Dispose()
    {
        _disposeCts.Cancel();
        GC.SuppressFinalize(this);
    }

    public async Task HandleAsync(Func<Task> onNext)
    {
        await _requestSemaphore.WaitAsync();
        await onNext();
    }
}
