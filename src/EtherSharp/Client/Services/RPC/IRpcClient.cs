using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.RPC;
public interface IRpcClient
{
    public Task<ulong> EthChainId();
    public Task<ulong> EthEstimateGasAsync(string from, string to, BigInteger value, string data);
    public Task<BigInteger> EthGasPriceAsync();
    public Task<BigInteger> EthMaxPriorityFeePerGas();
    public Task<BigInteger> EthGetBalance(string address, TargetBlockNumber blockNumber);
    public Task<TxCallResult> EthCallAsync(
        string? from, string to, uint? gas, BigInteger? gasPrice, int? value, string? data, TargetBlockNumber blockNumber);
    public Task<uint> EthGetTransactionCount(string address, TargetBlockNumber blockNumber);
    public Task<TxSubmissionResult> EthSendRawTransactionAsync(string transaction);
    public Task<TransactionReceipt?> EthGetTransactionReceiptAsync(string transactionHash);
}
