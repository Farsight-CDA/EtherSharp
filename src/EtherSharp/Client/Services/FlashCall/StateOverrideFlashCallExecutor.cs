using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class StateOverrideFlashCallExecutor(
    IEthRpcModule ethRpcModule,
    StateOverrideFlashCallExecutor.Configuration configuration) : IFlashRuntimeExecutor
{
    internal sealed record Configuration(int MaxPayloadSize, int MaxResultSize);

    private static readonly Address _flashCodeAddress = Address.Parse("0x4574686572536861727051756572696572000000");

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
        CancellationToken cancellationToken)
    {
        var stateOverrides = options.StateOverrides is null
            ? []
            : new Dictionary<Address, AccountOverride>(options.StateOverrides);

        if(stateOverrides.ContainsKey(_flashCodeAddress))
        {
            throw new InvalidOperationException($"Flash call state overrides conflict at {_flashCodeAddress}.");
        }

        stateOverrides.Add(
            _flashCodeAddress,
            new AccountOverride(
                balance: UInt256.Zero,
                nonce: 1,
                code: runtimeCode.ByteCode,
                state: new Dictionary<Bytes32, Bytes32>()
            )
        );

        return _ethRpcModule.CallAsync(
            _flashCodeAddress,
            flashCallGasLimit,
            null,
            call.Value,
            call.Data,
            options with { StateOverrides = stateOverrides },
            cancellationToken
        );
    }
}
