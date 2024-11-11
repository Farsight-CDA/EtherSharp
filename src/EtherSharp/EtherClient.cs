using EtherSharp.Core;
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

    public Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default) 
        => _evmRPCClient.EthGetBalance(address, targetHeight);

    public TContract Contract<TContract>(string address) where TContract : IContract 
        => throw new NotImplementedException();
}
