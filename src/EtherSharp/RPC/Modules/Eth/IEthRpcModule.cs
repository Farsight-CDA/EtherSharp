using EtherSharp.Client;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Eth;

/// <summary>
/// Low-level wrapper around the node <c>eth_*</c> JSON-RPC methods.
/// </summary>
public interface IEthRpcModule
{
    /// <summary>
    /// Gets the chain id.
    /// </summary>
    public Task<ulong> ChainIdAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest block number.
    /// </summary>
    public Task<ulong> BlockNumberAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates gas for a transaction call, optionally applying state and block overrides.
    /// </summary>
    public Task<ulong> EstimateGasAsync(
        Address? to, UInt256 value, ReadOnlyMemory<byte> data,
        in CallOptions options, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an access list for a transaction call at the requested block.
    /// </summary>
    public Task<AccessListResult> CreateAccessListAsync(
        Address? to, UInt256 value, ReadOnlyMemory<byte> data,
        CallOptions options, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a block by number with transaction hashes only.
    /// </summary>
    public Task<Block> GetBlockByNumberAsync(
        TargetHeight targetHeight, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a transaction by hash.
    /// </summary>
    public Task<TxData?> TransactionByHashAsync(
        in Bytes32 hash, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets fee history data.
    /// </summary>
    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetHeight newestBlock,
        double[] rewardPercentiles, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the current gas price.
    /// </summary>
    public Task<UInt256> GasPriceAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the suggested max priority fee per gas.
    /// </summary>
    public Task<UInt256> MaxPriorityFeePerGasAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets account balance at a target block.
    /// </summary>
    public Task<UInt256> GetBalanceAsync(
        in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a read-only call, optionally applying state and block overrides.
    /// </summary>
    public Task<TxCallResult> CallAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in CallOptions options,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transaction count (nonce) at a target block.
    /// </summary>
    public Task<uint> GetTransactionCountAsync(
        in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Broadcasts a signed raw transaction.
    /// </summary>
    public Task<TxSubmissionResult> SendRawTransactionAsync(
        string transaction, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a transaction receipt by transaction hash.
    /// </summary>
    public Task<TxReceipt?> GetTransactionReceiptAsync(
        in Bytes32 transactionHash, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Reads a contract storage slot.
    /// </summary>
    public Task<byte[]> GetStorageAtAsync(
        in Address address, byte[] slot, TargetHeight targetHeight = default, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets logs matching filter parameters.
    /// </summary>
    public Task<Log[]> GetLogsAsync(
        TargetHeight fromBlock, TargetHeight toBlock,
        EventFilter eventFilter, Bytes32? blockHash,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a log filter.
    /// </summary>
    public Task<string> NewFilterAsync(
        TargetHeight fromBlock, TargetHeight toBlock,
        EventFilter eventFilter,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets new log entries for a filter.
    /// </summary>
    public Task<Log[]> GetEventFilterChangesAsync(string filterId, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uninstalls a filter.
    /// </summary>
    public Task<bool> UninstallFilterAsync(string filterId, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to log notifications.
    /// </summary>
    public Task<string> SubscribeLogsAsync(
        EventFilter eventFilter, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Subscribes to new block headers.
    /// </summary>
    public Task<string> SubscribeNewHeadsAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an active subscription.
    /// </summary>
    public Task<bool> UnsubscribeAsync(string subscriptionId, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);
}
