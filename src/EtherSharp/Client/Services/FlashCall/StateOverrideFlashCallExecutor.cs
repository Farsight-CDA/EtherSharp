using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class StateOverrideFlashCallExecutor(
    IEthRpcModule ethRpcModule,
    StateOverrideFlashCallExecutor.Configuration configuration) : IFlashRuntimeExecutor
{
    internal sealed record Configuration(int MaxPayloadSize, int MaxResultSize);

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly Configuration _configuration = configuration;

    public int GetMaxPayloadSize(ulong? flashCallGasLimit, TargetHeight targetHeight)
        => _configuration.MaxPayloadSize;

    public int GetMaxResultSize(TargetHeight targetHeight)
        => _configuration.MaxResultSize;

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
