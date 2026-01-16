using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Eth;

public interface IEthRpcModule
{
    public ulong? DefaultCallGas { get; set; }

    public Task<ulong> ChainIdAsync(CancellationToken cancellationToken = default);
    public Task<ulong> BlockNumberAsync(CancellationToken cancellationToken = default);
    public Task<ulong> EstimateGasAsync(
        Address? from, Address? to, UInt256 value, string data,
        CancellationToken cancellationToken = default
    );

    public Task<BlockDataTrasactionAsString> GetBlockByNumberAsync(
        TargetBlockNumber targetHeight, CancellationToken cancellationToken
    );
    public Task<Transaction?> TransactionByHashAsync(
        string hash, CancellationToken cancellationToken
    );

    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken
    );
    public Task<UInt256> GasPriceAsync(CancellationToken cancellationToken = default);
    public Task<UInt256> MaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default);
    public Task<UInt256> GetBalanceAsync(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken = default
    );
    public Task<TxCallResult> CallAsync(
        Address? from, Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, string? data,
        TargetBlockNumber blockNumber, CancellationToken cancellationToken = default);
    public Task<uint> GetTransactionCountAsync(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken = default
    );
    public Task<TxSubmissionResult> SendRawTransactionAsync(
        string transaction, CancellationToken cancellationToken = default
    );
    public Task<TransactionReceipt?> GetTransactionReceiptAsync(
        string transactionHash, CancellationToken cancellationToken = default
    );

    public Task<byte[]> GetStorageAtAsync(
        Address address, byte[] slot, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default
    );

    public Task<Log[]> GetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken = default
    );
    public Task<string> NewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? address, string[]?[]? topics,
        CancellationToken cancellationToken = default
    );
    public Task<Log[]> GetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken = default);
    public Task<bool> UninstallFilterAsync(string filterId, CancellationToken cancellationToken = default);

    public Task<string> SubscribeLogsAsync(
        Address[]? contracts, string[]?[]? topics, CancellationToken cancellationToken = default
    );
    public Task<string> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default);
    public Task<bool> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
