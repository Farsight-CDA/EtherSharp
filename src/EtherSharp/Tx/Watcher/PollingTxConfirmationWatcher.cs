using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Implementation of <see cref="ITxConfirmationWatcher"/> that watches sender nonces through an
/// <see cref="INonceWatcher"/> and resolves each watched slot independently once its nonce is used.
/// </summary>
public sealed partial class PollingTxConfirmationWatcher : ITxConfirmationWatcher, IAsyncDisposable
{
    /// <summary>
    /// Configuration for <see cref="PollingTxConfirmationWatcher"/>.
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// Delay between nonce reads while at least one watch waits for its nonce to be used.
        /// </summary>
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Maximum backward distance in blocks probed via exponential lookback before falling back to transaction lookups by hash.
        /// </summary>
        public ulong MaxLookbackBlocks { get; set; } = 4096;

        /// <summary>
        /// Optional RPC request options applied to watcher RPC calls.
        /// </summary>
        public RpcRequestOptions RequestOptions { get; set; }
    }

    private readonly IEthRpcModule _ethRpcModule;
    private readonly INonceWatcher _nonceWatcher;
    private readonly ILogger<PollingTxConfirmationWatcher>? _logger;

    private readonly TimeSpan _pollInterval;
    private readonly ulong _maxLookbackBlocks;
    private readonly TimeSpan _retryDelay;
    private readonly TimeSpan _maxErrorBackoff;
    private readonly RpcRequestOptions _requestOptions;

    private readonly Lock _stateLock = new Lock();
    private readonly Dictionary<(Address Sender, ulong Nonce), SlotTracker> _slots = [];

    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="PollingTxConfirmationWatcher"/> backed by the supplied Ethereum RPC module.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query nonces, blocks, transactions, and receipts.</param>
    /// <param name="nonceWatcher">Watcher subscribed to for sender nonce updates.</param>
    /// <param name="options">Optional watcher configuration.</param>
    /// <param name="retryOptions">Optional retry and backoff configuration for transient RPC failures.</param>
    /// <param name="logger">Optional logger for transient RPC diagnostics.</param>
    public PollingTxConfirmationWatcher(
        IEthRpcModule ethRpcModule,
        INonceWatcher nonceWatcher,
        Options? options = null,
        RpcRetryOptions? retryOptions = null,
        ILogger<PollingTxConfirmationWatcher>? logger = null
    )
    {
        ArgumentNullException.ThrowIfNull(ethRpcModule);
        ArgumentNullException.ThrowIfNull(nonceWatcher);

        var resolved = options ?? new Options();
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(resolved.PollInterval, TimeSpan.Zero, nameof(options));
        ArgumentOutOfRangeException.ThrowIfZero(resolved.MaxLookbackBlocks, nameof(options));

        var resolvedRetryOptions = retryOptions ?? new RpcRetryOptions();
        resolvedRetryOptions.Validate(nameof(retryOptions));

        _ethRpcModule = ethRpcModule;
        _nonceWatcher = nonceWatcher;
        _logger = logger;

        _pollInterval = resolved.PollInterval;
        _maxLookbackBlocks = resolved.MaxLookbackBlocks;
        _retryDelay = resolvedRetryOptions.RetryDelay;
        _maxErrorBackoff = resolvedRetryOptions.MaxErrorBackoff;
        _requestOptions = resolved.RequestOptions;
    }

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
        if(!_slots.TryGetValue(key, out var slot))
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
