using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;

internal sealed class EtherModule(IEthRpcModule ethRpcModule) : IEtherModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public ITxInput Transfer(in Address receiver, UInt256 amount)
        => ITxInput.ForEthTransfer(in receiver, amount);
    public ITxInput Transfer(IPayableContract contract, UInt256 amount)
        => ITxInput.ForEthTransfer(contract.Address, amount);

    public Task<UInt256> GetBalanceAsync(in Address address, TargetHeight targetHeight,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => _ethRpcModule.GetBalanceAsync(in address, targetHeight, requestOptions, cancellationToken);
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetHeight targetHeight,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => _ethRpcModule.GetBalanceAsync(contract.Address, targetHeight, requestOptions, cancellationToken);
}
