using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.RPC.Modules.Eth;

public interface IEthRpcModule
{
    public Task<ulong> ChainIdAsync(CancellationToken cancellationToken = default);
    public Task<ulong> BlockNumberAsync(CancellationToken cancellationToken = default);
    public Task<ulong> EstimateGasAsync(
        Address? from, Address? to, BigInteger value, string data,
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
    public Task<BigInteger> GasPriceAsync(CancellationToken cancellationToken = default);
    public Task<BigInteger> MaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default);
    public Task<BigInteger> GetBalanceAsync(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken = default
    );
    public Task<TxCallResult> CallAsync(
        Address? from, Address? to, uint? gas, BigInteger? gasPrice, BigInteger value, string? data,
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
