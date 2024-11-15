using EtherSharp.Contract;
using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp;
public class EtherClient
{
    private readonly EvmRpcClient _evmRPCClient;

    internal EtherClient(EvmRpcClient evmRpcClient)
    {
        _evmRPCClient = evmRpcClient;
    }

    public Task<ulong> GetChainIdAsync()
        => _evmRPCClient.EthChainId();

    public Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default) 
        => _evmRPCClient.EthGetBalance(address, targetHeight);

    public Task<int> GetTransactionCount(string address, TargetBlockNumber targetHeight = default)
        => _evmRPCClient.EthGetTransactionCount(address, targetHeight);

    public TContract Contract<TContract>(string address) where TContract : IContract 
        => throw new NotImplementedException();

    public T CallAsync<T>(TxInput<T> call) => throw new NotImplementedException();
    public Task<TransactionReceipt> SendAsync<T>(TxInput<T> call) => throw new NotImplementedException();
}
