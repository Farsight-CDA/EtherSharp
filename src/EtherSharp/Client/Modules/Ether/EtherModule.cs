using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client.Modules.Ether;

internal sealed class EtherModule(IEthRpcModule ethRpcModule, IServiceProvider provider) : IEtherTxModule, IEtherModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly IServiceProvider _provider = provider;

    public ITxInput Transfer(in Address receiver, UInt256 amount)
        => ITxInput.ForEthTransfer(in receiver, amount);
    public ITxInput Transfer(IPayableContract contract, UInt256 amount)
        => ITxInput.ForEthTransfer(contract.Address, amount);

    public Task<UInt256> GetBalanceAsync(in Address address, TargetHeight targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(in address, targetHeight, cancellationToken);
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetHeight targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(contract.Address, targetHeight, cancellationToken);

    public Task<UInt256> GetMyBalanceAsync(TargetHeight targetHeight, CancellationToken cancellationToken)
        => _ethRpcModule.GetBalanceAsync(
            _provider.GetService<IEtherSigner>()?.Address ?? throw new InvalidOperationException("Client is not a tx client"),
            targetHeight,
            cancellationToken
        );
}
