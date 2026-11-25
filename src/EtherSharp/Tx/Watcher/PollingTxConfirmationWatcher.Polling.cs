using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx.Watcher;

public sealed partial class PollingTxConfirmationWatcher
{
    private static readonly TimeSpan _errorBackoffDelay = TimeSpan.FromSeconds(1);

    private TimeSpan NextErrorBackoffDelay(TimeSpan delay)
        => delay * 2 < _maxErrorBackoffDelay
            ? delay * 2
            : _maxErrorBackoffDelay;

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        var errorBackoffDelay = _errorBackoffDelay;
        while(true)
        {
            SlotTracker[] waiting;
            lock(_stateLock)
            {
                // Runs while any slot is registered, Watch restarts it for the next slot.
                if(_slots.Count == 0)
                {
                    _heartbeatTask = null;
                    return;
                }

                waiting = [.. _slots.Values.Where(s => s.IsWaitingForNonceLocked)];
            }

            if(waiting.Length > 0)
            {
                var requests = waiting
                    .GroupBy(s => s.Sender)
                    .Select(g => new INonceSource.Request(g.Key, g.Min(s => s.Nonce)))
                    .ToArray();

                try
                {
                    var (blockNumber, nonces) = await _nonceSource.ReadAsync(requests, cancellationToken);

                    lock(_stateLock)
                    {
                        foreach(var slot in waiting)
                        {
                            slot.ObserveNonceLocked(nonces[Array.FindIndex(requests, r => r.Sender == slot.Sender)], blockNumber);
                        }
                    }

                    errorBackoffDelay = _errorBackoffDelay;
                }
                catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
                {
                    _logger?.LogWarning(ex, "Heartbeat: Reading nonces of {SenderCount} senders failed", requests.Length);
                    await Task.Delay(errorBackoffDelay, cancellationToken);
                    errorBackoffDelay = NextErrorBackoffDelay(errorBackoffDelay);
                    continue;
                }
            }

            await Task.Delay(_pollInterval, cancellationToken);
        }
    }
}
