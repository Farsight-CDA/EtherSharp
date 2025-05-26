using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.StateOverride;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.RPC;
public interface IRpcClient
{
    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public Task<ulong> EthChainIdAsync(CancellationToken cancellationToken = default);
    public Task<ulong> EthBlockNumberAsync(CancellationToken cancellationToken = default);
    public Task<ulong> EthEstimateGasAsync(
        Address? from, Address to, BigInteger value, string data,
        CancellationToken cancellationToken = default
    );

    public Task<BlockDataTrasactionAsString> EthGetBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken
    );
    public Task<Transaction> EthTransactionByHash(
        string hash, CancellationToken cancellationToken
    );

    public Task<FeeHistory> EthGetFeeHistory(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken
    );
    public Task<BigInteger> EthGasPriceAsync(CancellationToken cancellationToken = default);
    public Task<BigInteger> EthMaxPriorityFeePerGas(CancellationToken cancellationToken = default);
    public Task<BigInteger> EthGetBalance(
        Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken = default
    );
    public Task<TxCallResult> EthCallAsync(
        Address? from, Address to, uint? gas, BigInteger? gasPrice, int? value, string? data,
        TargetBlockNumber blockNumber, TxStateOverride? stateOverride, CancellationToken cancellationToken = default);
    public Task<uint> EthGetTransactionCount(
        Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken = default
    );
    public Task<TxSubmissionResult> EthSendRawTransactionAsync(
        string transaction, CancellationToken cancellationToken = default
    );
    public Task<TransactionReceipt?> EthGetTransactionReceiptAsync(
        string transactionHash, CancellationToken cancellationToken = default
    );

    public Task<Log[]> EthGetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken = default
    );
    public Task<string> EthNewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? address, string[]?[]? topics,
        CancellationToken cancellationToken = default
    );
    public Task<Log[]> EthGetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken = default);
    public Task<bool> EthUninstallFilterAsync(string filterId, CancellationToken cancellationToken = default);

    public Task<string> EthSubscribeLogsAsync(
        Address[]? contracts, string[]?[]? topics, CancellationToken cancellationToken = default
    );
    public Task<string> EthSubscribeNewHeadsAsync(CancellationToken cancellationToken = default);

    public Task<bool> EthUnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
