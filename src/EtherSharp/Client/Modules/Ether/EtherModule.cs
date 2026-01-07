using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client.Modules.Ether;

internal class EtherModule(IEthRpcModule ethRpcModule, IServiceProvider provider) : IEtherTxModule, IEtherModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly IServiceProvider _provider = provider;

    public ITxInput Transfer(Address receiver, UInt256 amount)
        => ITxInput.ForEthTransfer(receiver, amount);
    public ITxInput Transfer(IPayableContract contract, UInt256 amount)
        => ITxInput.ForEthTransfer(contract.Address, amount);

    public Task<UInt256> GetBalanceAsync(Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(address, targetHeight, cancellationToken);
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(contract.Address, targetHeight, cancellationToken);

    public Task<UInt256> GetMyBalanceAsync(TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(
            _provider.GetService<IEtherSigner>()?.Address ?? throw new InvalidOperationException("Client is not a tx client"),
            targetHeight,
            cancellationToken
        );
}
