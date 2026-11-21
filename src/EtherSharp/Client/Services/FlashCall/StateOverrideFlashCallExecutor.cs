using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Runtime.InteropServices;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class StateOverrideFlashCallExecutor(
    IEthRpcModule ethRpcModule) : FlashCallExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public override Task<TxCallResult> ExecuteAsync(
        EVMRuntimeCode runtimeCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        var address = FlashCallEncoding.GetStateOverrideAddress(FlashCallEncoding.GetInitCodeHash(runtimeCode));
        var stateOverrides = options.StateOverrides is null
            ? []
            : new Dictionary<Address, AccountOverride>(options.StateOverrides);

        var flashCodeOverride = new AccountOverride(balance: UInt256.Zero, nonce: 1, code: runtimeCode.ByteCode);
        ref var existingOverride = ref CollectionsMarshal.GetValueRefOrAddDefault(
            stateOverrides, address, out bool exists
        );

        if(exists)
        {
            if(existingOverride != flashCodeOverride)
            {
                throw new InvalidOperationException($"Flash call state overrides conflict at {address}.");
            }
        }
        else
        {
            existingOverride = flashCodeOverride;
        }

        return _ethRpcModule.CallAsync(
            address,
            null,
            call.Value,
            call.Data,
            options with { StateOverrides = stateOverrides },
            requestOptions,
            cancellationToken
        );
    }
}
