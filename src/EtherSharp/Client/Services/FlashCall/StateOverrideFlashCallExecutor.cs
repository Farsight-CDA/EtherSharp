using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Runtime.InteropServices;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class StateOverrideFlashCallExecutor(
    IEthRpcModule ethRpcModule) : IFlashRuntimeExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public Task<TxCallResult> ExecuteFlashCallAsync(
        EVMByteCode runtimeCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        var stateOverrides = options.StateOverrides is null
            ? []
            : new Dictionary<Address, AccountOverride>(options.StateOverrides);

        var flashCodeOverride = new AccountOverride(balance: UInt256.Zero, nonce: 1, code: runtimeCode.ByteCode);
        ref var existingOverride = ref CollectionsMarshal.GetValueRefOrAddDefault(
            stateOverrides, IQuerier.StateOverride.Address, out bool exists
        );

        if(exists)
        {
            if(existingOverride != flashCodeOverride)
            {
                throw new InvalidOperationException($"Flash call state overrides conflict at {IQuerier.StateOverride.Address}.");
            }
        }
        else
        {
            existingOverride = flashCodeOverride;
        }

        return _ethRpcModule.CallAsync(
            IQuerier.StateOverride.Address,
            null,
            call.Value,
            call.Data,
            options with { StateOverrides = stateOverrides },
            requestOptions,
            cancellationToken
        );
    }
}
