using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

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
        if(stateOverrides.TryGetValue(IQuerier.StateOverride.Address, out var existingOverride))
        {
            if(existingOverride != flashCodeOverride)
            {
                throw new InvalidOperationException($"Flash call state overrides conflict at {IQuerier.StateOverride.Address}.");
            }
        }
        else
        {
            stateOverrides.Add(IQuerier.StateOverride.Address, flashCodeOverride);
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
