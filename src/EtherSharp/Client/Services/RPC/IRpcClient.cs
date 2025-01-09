using EtherSharp.Events.Subscription;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.RPC;
public interface IRpcClient
{
    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public Task<ulong> EthChainIdAsync(CancellationToken cancellationToken = default);
    public Task<long> EthBlockNumberAsync(CancellationToken cancellationToken = default);
    public Task<ulong> EthEstimateGasAsync(
        string from, string to, BigInteger value, string data, CancellationToken cancellationToken = default
    );

    public Task<BlockDataTrasactionAsString> EthGetBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken);

    public Task<BigInteger> EthGasPriceAsync(CancellationToken cancellationToken = default);
    public Task<BigInteger> EthMaxPriorityFeePerGas(CancellationToken cancellationToken = default);
    public Task<BigInteger> EthGetBalance(
        string address, TargetBlockNumber blockNumber, CancellationToken cancellationToken = default
    );
    public Task<TxCallResult> EthCallAsync(
        string? from, string to, uint? gas, BigInteger? gasPrice, int? value, string? data, TargetBlockNumber blockNumber, 
        CancellationToken cancellationToken = default);
    public Task<uint> EthGetTransactionCount(
        string address, TargetBlockNumber blockNumber, CancellationToken cancellationToken = default
    );
    public Task<TxSubmissionResult> EthSendRawTransactionAsync(
        string transaction, CancellationToken cancellationToken = default
    );
    public Task<TransactionReceipt?> EthGetTransactionReceiptAsync(
        string transactionHash, CancellationToken cancellationToken = default
    );

    public Task<Log[]> EthGetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? addresses, string[]? topics, byte[]? blockHash,
        CancellationToken cancellationToken = default
    );
    public Task<string> EthNewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? address, string[]? topics,
        CancellationToken cancellationToken = default
    );
    public Task<Log[]> EthGetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken = default);
    public Task<bool> EthUninstallFilterAsync(string filterId, CancellationToken cancellationToken = default);

    public Task<string> EthSubscribeLogsAsync(
        string[]? contracts, string[]? topics, CancellationToken cancellationToken = default
    ); 
    public Task<bool> EthUnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
