using EtherSharp.Client.Services.RPC;
using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
internal class EtherApi(IRpcClient rpcClient, IServiceProvider provider) : IEtherTxApi
{
    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IServiceProvider _provider = provider;

    public ITxInput Transfer(Address receiver, BigInteger amount)
        => ITxInput.ForEthTransfer(receiver, amount);
    public ITxInput Transfer(IPayableContract contract, BigInteger amount)
        => ITxInput.ForEthTransfer(contract.Address, amount);

    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(address, blockNumber);
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(contract.Address, blockNumber);

    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber blockNumber)
        => _rpcClient.EthGetBalance(
            _provider.GetService<IEtherSigner>()?.Address ?? throw new InvalidOperationException("Client is not a tx client"),
            blockNumber
        );
}
