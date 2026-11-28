using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx.Watcher;

internal sealed partial class PollingTxConfirmationWatcher(
    IEthRpcModule ethRpcModule,
    INonceWatcher nonceWatcher,
    RpcRetryOptions retryOptions,
    ILoggerFactory? loggerFactory = null
    ) : ITxConfirmationWatcher, IAsyncDisposable
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly INonceWatcher _nonceWatcher = nonceWatcher;
    private readonly ILogger<PollingTxConfirmationWatcher>? _logger = loggerFactory?.CreateLogger<PollingTxConfirmationWatcher>();

    private readonly TimeSpan _retryDelay = retryOptions.RetryDelay;
    private readonly TimeSpan _maxErrorBackoff = retryOptions.MaxErrorBackoff;

    private readonly Lock _stateLock = new Lock();
    private readonly Dictionary<(Address Sender, ulong Nonce), SlotTracker> _slots = [];
    private readonly Dictionary<Bytes32, SlotTracker> _txHashSlots = [];

    private bool _isDisposed;

    /// <inheritdoc/>
    public ITxWatch Watch(in Address sender, ulong nonce, params ReadOnlySpan<Bytes32> txHashes)
        => Watch(in sender, nonce, null, txHashes);

    /// <inheritdoc/>
    public ITxWatch Watch(in Address sender, ulong nonce, ulong unusedAtBlock, params ReadOnlySpan<Bytes32> txHashes)
        => Watch(in sender, nonce, (ulong?) unusedAtBlock, txHashes);

    private TxWatchHandle Watch(in Address sender, ulong nonce, ulong? unusedAtBlock, ReadOnlySpan<Bytes32> txHashes)
    {
        using var scope = _stateLock.EnterScope();
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var key = (sender, nonce);
        _ = _slots.TryGetValue(key, out var slot);

        foreach(var txHash in txHashes)
        {
            ThrowIfTrackedByOtherSlotLocked(in txHash, slot);
        }

        if(slot is null)
        {
            slot = new SlotTracker(this, sender, nonce);
            _slots.Add(key, slot);
            slot.Start(cancellationToken => RunSlotAsync(slot, unusedAtBlock, cancellationToken));
        }

        var handle = new TxWatchHandle(slot);
        slot.Handles.Add(handle);
        foreach(var txHash in txHashes)
        {
            _ = slot.TryAddHash(handle, in txHash);
        }

        return handle;
    }

    private void ThrowIfTrackedByOtherSlotLocked(in Bytes32 txHash, SlotTracker? slot)
    {
        if(_txHashSlots.TryGetValue(txHash, out var owner) && owner != slot)
        {
            throw new InvalidOperationException(
                $"Transaction hash {txHash} is already watched for {owner.Sender}/{owner.Nonce}."
            );
        }
    }

    /// <inheritdoc/>
    public void NotifyIncluded(ulong blockNumber, in Bytes32 blockHash, ReadOnlySpan<Bytes32> transactionHashes)
    {
        // Read without the lock: a stale count only skips hashes registered concurrently, which polling still resolves.
        if(_txHashSlots.Count == 0)
        {
            return;
        }

        using var scope = _stateLock.EnterScope();

        if(_isDisposed)
        {
            return;
        }

        foreach(var txHash in transactionHashes)
        {
            if(_txHashSlots.TryGetValue(txHash, out var slot))
            {
                slot.Resolve(new TxWatchResult.Confirmed(txHash, blockNumber, blockHash));
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        List<Task> tasks = [];

        lock(_stateLock)
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach(var slot in _slots.Values)
            {
                foreach(var handle in slot.Handles)
                {
                    handle.Abort();
                }

                tasks.Add(slot.CancelAsync());
            }

            _slots.Clear();
            _txHashSlots.Clear();
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch(OperationCanceledException)
        {
        }
        catch(Exception ex)
        {
            _logger?.TxWatcher.InFlightWorkShutdownFailed(ex);
        }
    }
}
