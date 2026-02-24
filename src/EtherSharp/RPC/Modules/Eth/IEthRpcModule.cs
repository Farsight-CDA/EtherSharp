using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Eth;

/// <summary>
/// Low-level wrapper around the node <c>eth_*</c> JSON-RPC methods.
/// </summary>
public interface IEthRpcModule
{
    /// <summary>
    /// Optional fallback gas value applied to <c>eth_call</c> when no explicit gas is provided.
    /// </summary>
    public ulong? DefaultCallGas { get; set; }

    /// <summary>
    /// Gets the chain id.
    /// </summary>
    public Task<ulong> ChainIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest block number.
    /// </summary>
    public Task<ulong> BlockNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates gas for a transaction call.
    /// </summary>
    public Task<ulong> EstimateGasAsync(
        Address? from, Address? to, UInt256 value, string data,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a block by number with transaction hashes only.
    /// </summary>
    public Task<BlockDataTransactionAsString> GetBlockByNumberAsync(
        TargetBlockNumber targetHeight, CancellationToken cancellationToken
    );

    /// <summary>
    /// Gets a transaction by hash.
    /// </summary>
    public Task<Transaction?> TransactionByHashAsync(
        string hash, CancellationToken cancellationToken
    );

    /// <summary>
    /// Gets fee history data.
    /// </summary>
    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken
    );

    /// <summary>
    /// Gets the current gas price.
    /// </summary>
    public Task<UInt256> GasPriceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the suggested max priority fee per gas.
    /// </summary>
    public Task<UInt256> MaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets account balance at a target block.
    /// </summary>
    public Task<UInt256> GetBalanceAsync(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a read-only call.
    /// </summary>
    public Task<TxCallResult> CallAsync(
        Address? from, Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, string? data,
        TargetBlockNumber blockNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transaction count (nonce) at a target block.
    /// </summary>
    public Task<uint> GetTransactionCountAsync(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Broadcasts a signed raw transaction.
    /// </summary>
    public Task<TxSubmissionResult> SendRawTransactionAsync(
        string transaction, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a transaction receipt by transaction hash.
    /// </summary>
    public Task<TransactionReceipt?> GetTransactionReceiptAsync(
        string transactionHash, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Reads a contract storage slot.
    /// </summary>
    public Task<byte[]> GetStorageAtAsync(
        Address address, byte[] slot, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets logs matching filter parameters.
    /// </summary>
    public Task<Log[]> GetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a log filter.
    /// </summary>
    public Task<string> NewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? address, string[]?[]? topics,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets new log entries for a filter.
    /// </summary>
    public Task<Log[]> GetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uninstalls a filter.
    /// </summary>
    public Task<bool> UninstallFilterAsync(string filterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to log notifications.
    /// </summary>
    public Task<string> SubscribeLogsAsync(
        Address[]? contracts, string[]?[]? topics, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Subscribes to new block headers.
    /// </summary>
    public Task<string> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an active subscription.
    /// </summary>
    public Task<bool> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
