using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class DeployedFlashCallExecutor(
    IEthRpcModule ethRpcModule,
    CallGasLimitSettings callGasLimitSettings
) : FlashCallExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule, callGasLimitSettings);

    private ulong? _deploymentHeight;

    internal void Initialize(ulong chainId)
        => _deploymentHeight = IFlashCaller.GetDeploymentHeight(chainId);

    public override async Task<TxCallResult> ExecuteAsync(
        EVMRuntimeCode runtimeCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        if(options.StateOverrides is { Count: > 0 })
        {
            throw new NotSupportedException(
                $"State overrides in queries and flash calls require {nameof(EtherClientBuilder.WithStateOverrides)}()."
            );
        }

        flashCallGasLimit ??= _callGasLimitSettings.GetFlashCallGasLimit();

        var targetHeight = options.TargetHeight;
        if(_deploymentHeight is not { } deploymentHeight || targetHeight.Value < deploymentHeight || targetHeight == TargetHeight.Earliest)
        {
            return await _constructorFlashCallExecutor.ExecuteAsync(
                runtimeCode, call, flashCallGasLimit, options, requestOptions, cancellationToken
            );
        }

        var initCode = EVMInitCode.FromRuntimeCode(runtimeCode);
        int argsLength = DeployedFlashCallEncoding.GetPayloadLength(initCode, call.Data.Length);

        byte[] rented = ArrayPool<byte>.Shared.Rent(argsLength);
        var payload = rented.AsMemory(0, argsLength);

        try
        {
            DeployedFlashCallEncoding.Write(payload.Span, initCode, call.Data.Span, flashCallGasLimit);

            var result = await _ethRpcModule.CallAsync(
                IFlashCaller.Address,
                null,
                call.Value,
                payload,
                options,
                requestOptions,
                cancellationToken
            );

            return !result.Success
                ? throw CallRevertedException.Parse(IFlashCaller.Address, result.Data.Span)
                : DeployedFlashCallEncoding.DecodeResult(result.Data);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
