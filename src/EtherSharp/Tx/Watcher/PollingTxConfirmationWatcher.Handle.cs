using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

public sealed partial class PollingTxConfirmationWatcher
{
    private sealed class SlotHashEntry(Bytes32 hash)
    {
        public Bytes32 Hash { get; } = hash;
        public int RefCount { get; set; }
    }

    private sealed class SlotTracker(PollingTxConfirmationWatcher watcher, Address sender, ulong nonce)
    {
        public PollingTxConfirmationWatcher Watcher { get; } = watcher;
        public Address Sender { get; } = sender;
        public ulong Nonce { get; } = nonce;
        public List<TxWatchHandle> Handles { get; } = [];
        public List<SlotHashEntry> Hashes { get; } = [];

        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private Task _runTask = Task.CompletedTask;

        public void Start(Func<CancellationToken, Task> run)
            => _runTask = Task.Run(() => run(_cancellation.Token));

        public Task CancelAsync()
        {
            // Flags the token synchronously but runs its callbacks on the thread pool, so this is safe under the lock.
            _ = _cancellation.CancelAsync();
            return _runTask;
        }

        public bool TryAddHash(TxWatchHandle handle, in Bytes32 txHash)
        {
            using var scope = Watcher._stateLock.EnterScope();

            if(Watcher._isDisposed || handle.IsCompleted)
            {
                return false;
            }

            var entry = FindHashLocked(in txHash);
            if(entry is null)
            {
                Watcher.ThrowIfTrackedByOtherSlotLocked(in txHash, this);

                entry = new SlotHashEntry(txHash);
                Hashes.Add(entry);
                Watcher._txHashSlots.Add(txHash, this);
            }
            else if(handle.Hashes.Contains(entry))
            {
                return true;
            }

            entry.RefCount++;
            handle.Hashes.Add(entry);
            return true;
        }

        public bool TryRemoveHash(TxWatchHandle handle, in Bytes32 txHash)
        {
            using var scope = Watcher._stateLock.EnterScope();

            if(Watcher._isDisposed || handle.IsCompleted)
            {
                return false;
            }

            foreach(var entry in handle.Hashes)
            {
                if(entry.Hash == txHash)
                {
                    _ = handle.Hashes.Remove(entry);
                    ReleaseHashLocked(entry);
                    return true;
                }
            }

            return false;
        }

        public void Resolve(TxWatchResult.Confirmed? inclusion)
        {
            using var scope = Watcher._stateLock.EnterScope();

            // A slot without handles was abandoned and may already be replaced by a new slot for the same key.
            if(Watcher._isDisposed || Handles.Count == 0)
            {
                return;
            }

            _ = Watcher._slots.Remove((Sender, Nonce));

            var winningEntry = inclusion is not null
                ? FindHashLocked(inclusion.TransactionHash)
                : null;
            foreach(var handle in Handles)
            {
                TxWatchResult result = inclusion is null
                    ? new TxWatchResult.NonceAlreadyUsed()
                    : winningEntry is not null && handle.Hashes.Contains(winningEntry)
                        ? inclusion
                        : new TxWatchResult.Replaced(inclusion.TransactionHash, inclusion.BlockNumber, inclusion.BlockHash);

                handle.Complete(result);
            }

            Handles.Clear();

            foreach(var entry in Hashes)
            {
                _ = Watcher._txHashSlots.Remove(entry.Hash);
            }
        }

        public void Unregister(TxWatchHandle handle)
        {
            using var scope = Watcher._stateLock.EnterScope();

            if(Watcher._isDisposed || handle.IsCompleted)
            {
                return;
            }

            handle.Abort();

            foreach(var entry in handle.Hashes)
            {
                ReleaseHashLocked(entry);
            }

            _ = Handles.Remove(handle);
            if(Handles.Count > 0)
            {
                return;
            }

            _ = Watcher._slots.Remove((Sender, Nonce));
            _ = CancelAsync();
        }

        private void ReleaseHashLocked(SlotHashEntry entry)
        {
            entry.RefCount--;
            if(entry.RefCount == 0)
            {
                _ = Hashes.Remove(entry);
                _ = Watcher._txHashSlots.Remove(entry.Hash);
            }
        }

        private SlotHashEntry? FindHashLocked(in Bytes32 hash)
        {
            foreach(var entry in Hashes)
            {
                if(entry.Hash == hash)
                {
                    return entry;
                }
            }

            return null;
        }
    }

    private sealed class TxWatchHandle(SlotTracker slot) : ITxWatch
    {
        private readonly TaskCompletionSource<TxWatchResult> _tcs
            = new TaskCompletionSource<TxWatchResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        public List<SlotHashEntry> Hashes { get; } = [];
        public bool IsCompleted => _tcs.Task.IsCompleted;

        Address ITxWatch.Sender => slot.Sender;
        ulong ITxWatch.Nonce => slot.Nonce;

        bool ITxWatch.TryAddHash(in Bytes32 txHash)
            => slot.TryAddHash(this, in txHash);

        bool ITxWatch.TryRemoveHash(in Bytes32 txHash)
            => slot.TryRemoveHash(this, in txHash);

        Task<TxWatchResult> ITxWatch.WaitAsync(CancellationToken cancellationToken)
            => _tcs.Task.WaitAsync(cancellationToken);

        void IDisposable.Dispose()
            => slot.Unregister(this);

        public void Complete(TxWatchResult result)
            => _tcs.SetResult(result);

        public void Abort()
        {
            _tcs.SetException(new ObjectDisposedException(nameof(ITxWatch)));
            _ = _tcs.Task.Exception;
        }
    }
}
