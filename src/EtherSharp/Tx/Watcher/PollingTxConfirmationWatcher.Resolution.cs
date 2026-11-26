using EtherSharp.Common.Exceptions;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx.Watcher;

public sealed partial class PollingTxConfirmationWatcher
{
    private const ulong INITIAL_LOOKBACK_STEP = 16;
    private const int HASH_LOOKUP_RETRY_COUNT = 3;
    private const int BLOCK_FETCH_RETRY_COUNT = 3;
    private const int FAILED_SEARCH_LIMIT = 2;

    private async Task RunSlotAsync(SlotTracker slot, CancellationToken cancellationToken)
    {
        int failedSearchCount = 0;
        var errorBackoffDelay = _errorBackoffDelay;
        while(true)
        {
            var (lowBlock, observedBlock) = await slot.WaitForNonceUsedAsync(cancellationToken);

            try
            {
                var search = await FindMinedBlockAsync(slot, lowBlock, observedBlock, cancellationToken);
                if(search is not MinedBlockResult.Found(var minedBlock))
                {
                    // A transaction older than the lookback range is served by every node, so retrying cannot help.
                    await ResolveByHashAsync(slot, search is MinedBlockResult.BeyondLookback ? 0 : HASH_LOOKUP_RETRY_COUNT, cancellationToken);
                    return;
                }

                if(await FetchMinedBlockAsync(minedBlock, cancellationToken) is not { } block)
                {
                    // A desynced or reorging node, which says nothing about the slot, so the next heartbeat starts a fresh search.
                    _logger?.LogWarning(
                        "Slot {Sender}/{Nonce}: Block {BlockNumber} unavailable after retries, waiting for the nonce again",
                        slot.Sender, slot.Nonce, minedBlock
                    );
                    continue;
                }

                if(Array.Find(block.Transactions, tx => tx.From == slot.Sender && tx.Nonce == slot.Nonce)
                    is { BlockNumber: ulong blockNumber, BlockHash: Bytes32 blockHash } match)
                {
                    slot.Resolve(new TxWatchResult.Confirmed(match.Hash, blockNumber, blockHash));
                    return;
                }

                // E.g. after a reorg, or when the nonce was used by an EIP-7702 authorization instead of a transaction.
                _logger?.LogWarning(
                    "Slot {Sender}/{Nonce}: Block {BlockNumber} has no matching transaction",
                    slot.Sender, slot.Nonce, minedBlock
                );
            }
            catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
            {
                // An error leaves the search unfinished, so it does not count as a failed search.
                _logger?.LogWarning(ex, "Slot {Sender}/{Nonce}: Search failed", slot.Sender, slot.Nonce);
                await Task.Delay(errorBackoffDelay, cancellationToken);
                errorBackoffDelay = NextErrorBackoffDelay(errorBackoffDelay);
                continue;
            }

            errorBackoffDelay = _errorBackoffDelay;

            // A reorg is found by the next search, a block that keeps lacking the transaction will not change.
            failedSearchCount++;
            if(failedSearchCount >= FAILED_SEARCH_LIMIT)
            {
                _logger?.LogWarning(
                    "Slot {Sender}/{Nonce}: Unconfirmed after {SearchCount} searches, treating the nonce as used",
                    slot.Sender, slot.Nonce, failedSearchCount
                );
                slot.Resolve(null);
                return;
            }
        }
    }

    private closed record MinedBlockResult
    {
        public sealed record Found(ulong BlockNumber) : MinedBlockResult;

        public sealed record BeyondLookback : MinedBlockResult;

        public sealed record Unavailable : MinedBlockResult;
    }

    private async Task<MinedBlockResult> FindMinedBlockAsync(
        SlotTracker slot,
        ulong? highestUnusedBlock,
        ulong observedBlock,
        CancellationToken cancellationToken
    )
    {
        ulong lowBlock;
        ulong highBlock = observedBlock;

        // An unused block at or after the observed one points to diverging replicas, a reorg or a wrong seed,
        // one further back than the lookback range to an old seed whose state a node may no longer serve.
        if(highestUnusedBlock is { } unused && unused < observedBlock && observedBlock - unused <= _maxLookbackBlocks)
        {
            lowBlock = unused;
        }
        else
        {
            var level = highestUnusedBlock is null ? LogLevel.Information : LogLevel.Warning;
            if(_logger?.IsEnabled(level) == true)
            {
                _logger.Log(
                    level,
                    "Slot {Sender}/{Nonce}: Used at block {ObservedBlock} without usable unused block {UnusedBlock}, looking back",
                    slot.Sender, slot.Nonce, observedBlock, highestUnusedBlock
                );
            }

            LookbackResult lookback;
            try
            {
                lookback = await LookBackAsync(slot, observedBlock, cancellationToken);
            }
            catch(RpcErrorException ex)
            {
                // E.g. pruned historical state, or a lagging node that does not serve the block yet.
                _logger?.LogWarning(
                    ex,
                    "Slot {Sender}/{Nonce}: Lookback failed, looking up hashes",
                    slot.Sender, slot.Nonce
                );
                return new MinedBlockResult.Unavailable();
            }

            if(lookback is not LookbackResult.Found(var foundLowBlock, var foundHighBlock))
            {
                _logger?.LogWarning(
                    "Slot {Sender}/{Nonce}: Used more than {MaxLookbackBlocks} blocks before block {ObservedBlock}, looking up hashes",
                    slot.Sender, slot.Nonce, _maxLookbackBlocks, observedBlock
                );
                return new MinedBlockResult.BeyondLookback();
            }

            (lowBlock, highBlock) = (foundLowBlock, foundHighBlock);
        }

        while(highBlock - lowBlock > 1)
        {
            ulong midBlock = lowBlock + ((highBlock - lowBlock) / 2);
            ulong midNonce = await _ethRpcModule.GetTransactionCountAsync(
                slot.Sender, TargetHeight.Height(midBlock), _requestOptions, cancellationToken
            );

            if(midNonce <= slot.Nonce)
            {
                lowBlock = midBlock;
            }
            else
            {
                highBlock = midBlock;
            }
        }

        return new MinedBlockResult.Found(highBlock);
    }

    // Returns the block, or null when it stays unavailable after the retries.
    private async Task<DetailedBlockData?> FetchMinedBlockAsync(ulong blockNumber, CancellationToken cancellationToken)
    {
        var retryDelay = _retryDelay;
        for(int attempt = 0; true; attempt++)
        {
            var block = await _ethRpcModule.GetFullBlockByNumberAsync(
                TargetHeight.Height(blockNumber), _requestOptions, cancellationToken
            );
            if(block is not null || attempt == BLOCK_FETCH_RETRY_COUNT)
            {
                return block;
            }

            // A lagging node may not serve the block yet.
            await Task.Delay(retryDelay, cancellationToken);
            retryDelay *= 2;
        }
    }

    private closed record LookbackResult
    {
        public sealed record Found(ulong LowBlock, ulong HighBlock) : LookbackResult;

        public sealed record OutOfRange : LookbackResult;
    }

    // Probes backwards with growing steps for a block where the nonce was still unused.
    private async Task<LookbackResult> LookBackAsync(
        SlotTracker slot,
        ulong observedBlock,
        CancellationToken cancellationToken
    )
    {
        ulong highBlock = observedBlock;
        ulong distance = Math.Min(INITIAL_LOOKBACK_STEP, _maxLookbackBlocks);
        while(true)
        {
            ulong probeBlock = observedBlock > distance ? observedBlock - distance : 0;
            ulong probeNonce = await _ethRpcModule.GetTransactionCountAsync(
                slot.Sender, TargetHeight.Height(probeBlock), _requestOptions, cancellationToken
            );

            if(probeNonce <= slot.Nonce)
            {
                return new LookbackResult.Found(probeBlock, highBlock);
            }

            if(probeBlock == 0 || distance >= _maxLookbackBlocks)
            {
                return new LookbackResult.OutOfRange();
            }

            highBlock = probeBlock;
            distance = distance > (_maxLookbackBlocks / 4)
                ? _maxLookbackBlocks
                : distance * 4;
        }
    }

    private async Task ResolveByHashAsync(SlotTracker slot, int retryCount, CancellationToken cancellationToken)
    {
        var retryDelay = _retryDelay;
        for(int attempt = 0; true; attempt++)
        {
            SlotHashEntry[] hashes;
            lock(_stateLock)
            {
                hashes = [.. slot.Hashes];
            }

            for(int i = hashes.Length - 1; i >= 0; i--)
            {
                var hash = hashes[i].Hash;
                var tx = await _ethRpcModule.TransactionByHashAsync(in hash, _requestOptions, cancellationToken);
                if(tx is not { BlockNumber: ulong blockNumber, BlockHash: Bytes32 blockHash })
                {
                    continue;
                }

                // A hash registered on the wrong watch must not confirm this slot.
                if(tx.From != slot.Sender || tx.Nonce != slot.Nonce)
                {
                    _logger?.LogWarning(
                        "Slot {Sender}/{Nonce}: Hash {TxHash} belongs to {TxSender}/{TxNonce}, ignoring it",
                        slot.Sender, slot.Nonce, hash, tx.From, tx.Nonce
                    );
                    continue;
                }

                slot.Resolve(new TxWatchResult.Confirmed(hash, blockNumber, blockHash));
                return;
            }

            if(hashes.Length == 0 || attempt == retryCount)
            {
                slot.Resolve(null);
                return;
            }

            // A lagging node may not serve the transaction yet.
            await Task.Delay(retryDelay, cancellationToken);
            retryDelay *= 2;
        }
    }
}
