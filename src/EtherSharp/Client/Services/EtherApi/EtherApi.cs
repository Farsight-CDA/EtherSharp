using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
internal class EtherApi(EvmRpcClient rpcClient) : IEtherTxApi
{
    private readonly EvmRpcClient _rpcClient = rpcClient;

    public Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(address, blockNumber);
    public TxInput Transfer(string receiver, BigInteger amount)
        => TxInput.ForEthTransfer(Address.FromString(receiver), amount);
}
