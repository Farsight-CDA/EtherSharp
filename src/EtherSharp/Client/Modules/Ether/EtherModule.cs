using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace EtherSharp.Client.Modules.Ether;

internal class EtherModule(IEthRpcModule ethRpcModule, IServiceProvider provider) : IEtherTxModule, IEtherModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly IServiceProvider _provider = provider;

    public ITxInput Transfer(Address receiver, BigInteger amount)
        => ITxInput.ForEthTransfer(receiver, amount);
    public ITxInput Transfer(IPayableContract contract, BigInteger amount)
        => ITxInput.ForEthTransfer(contract.Address, amount);

    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(address, targetBlockNumber, cancellationToken);
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(contract.Address, targetBlockNumber, cancellationToken);

    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(
            _provider.GetService<IEtherSigner>()?.Address ?? throw new InvalidOperationException("Client is not a tx client"),
            targetBlockNumber,
            cancellationToken
        );
}
