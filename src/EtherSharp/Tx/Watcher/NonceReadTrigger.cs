using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Common.Extensions;
using EtherSharp.Realtime;
using EtherSharp.Realtime.Blocks;
using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC.Transport;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EtherSharp.Tx.Watcher;

internal closed class NonceReadTrigger : IAsyncDisposable
{
    private static readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan _minHeadSpacing = TimeSpan.FromMilliseconds(500);

    public static async Task<NonceReadTrigger> CreateAsync(
        IRPCTransport transport,
        IBlocksModule blocksModule,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        if(!transport.SupportsSubscriptions)
        {
            return new Interval(_pollInterval);
        }

        try
        {
            var subscription = await blocksModule.SubscribeNewHeadsAsync(cancellationToken: cancellationToken);
            return new NewHeads(subscription, _minHeadSpacing);
        }
        catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
        {
            logger?.TxWatcher.NewHeadsUnavailable(ex, _pollInterval);
            return new Interval(_pollInterval);
        }
    }

    public sealed class Interval(TimeSpan interval) : NonceReadTrigger
    {
        private readonly TimeSpan _interval = interval > TimeSpan.Zero
            ? interval
            : throw new ArgumentOutOfRangeException(nameof(interval));

        public override Task WaitAsync(CancellationToken cancellationToken)
            => Task.Delay(_interval, cancellationToken);
    }

    public sealed class NewHeads : NonceReadTrigger
    {
        private readonly IBlocksSubscription _subscription;
        private readonly TimeSpan _minSpacing;

        private readonly SemaphoreSlim _headSignal = new SemaphoreSlim(0, 1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _listenTask;

        private long _lastSignalTimestamp;

        public NewHeads(IBlocksSubscription subscription, TimeSpan minSpacing)
        {
            _subscription = subscription;
            _minSpacing = minSpacing;
            _listenTask = Task.Run(() => ListenAsync(_cts.Token), CancellationToken.None);
        }

        public override async Task WaitAsync(CancellationToken cancellationToken)
        {
            var remaining = _minSpacing - Stopwatch.GetElapsedTime(_lastSignalTimestamp);
            if(remaining > TimeSpan.Zero)
            {
                await Task.Delay(remaining, cancellationToken);
            }

            await _headSignal.WaitAsync(cancellationToken);
            _lastSignalTimestamp = Stopwatch.GetTimestamp();
        }

        private async Task ListenAsync(CancellationToken cancellationToken)
        {
            await foreach(var subscriptionEvent in _subscription.ListenAsync(cancellationToken))
            {
                bool isReadDue = subscriptionEvent switch
                {
                    BlockHeader => true,
                    SubscriptionResumed => true,
                    SubscriptionInterrupted => false,
                };

                if(isReadDue)
                {
                    SignalHead();
                }
            }
        }

        private void SignalHead()
        {
            if(_headSignal.CurrentCount == 0)
            {
                _ = _headSignal.Release();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await _cts.CancelAsync();
            await _listenTask.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            await _subscription.DisposeAsync();

            _cts.Dispose();
            _headSignal.Dispose();
        }
    }

    /// <summary>
    /// Waits until the next read is due. Called after each read completes, and only while a subscription is active.
    /// </summary>
    public abstract Task WaitAsync(CancellationToken cancellationToken);

    /// <inheritdoc/>
    public virtual ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
