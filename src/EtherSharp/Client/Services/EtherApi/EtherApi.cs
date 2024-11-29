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
    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(address.String, blockNumber);
    public TxInput Transfer(string receiver, BigInteger amount)
        => TxInput.ForEthTransfer(Address.FromString(receiver), amount);
    public TxInput Transfer(Address receiver, BigInteger amount)
        => TxInput.ForEthTransfer(receiver, amount);
}
