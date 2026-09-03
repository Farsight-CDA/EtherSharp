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
    DeployedFlashCallExecutor.Configuration configuration) : IFlashInitCodeExecutor
{
    internal sealed record Configuration(
        Address ContractAddress,
        bool AllowFallback
    );

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly Configuration _configuration = configuration;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule);

    private ulong? _deploymentHeight;

    public Address ContractAddress => _configuration.ContractAddress;

    public void SetDeploymentHeight(ulong deploymentHeight)
        => _deploymentHeight = deploymentHeight;

    public async Task<TxCallResult> ExecuteFlashCallAsync(
        EVMByteCode initCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        var targetHeight = options.TargetHeight;

        if(_deploymentHeight is null)
        {
            return await _constructorFlashCallExecutor.ExecuteFlashCallAsync(
                initCode, call, flashCallGasLimit, options, requestOptions, cancellationToken);
        }

        if(targetHeight.IsNumeric && targetHeight.Value < _deploymentHeight.Value)
        {
            return !_configuration.AllowFallback
                ? throw new InvalidOperationException($"Missing FlashCall contract deployment at height {targetHeight.Value}")
                : await _constructorFlashCallExecutor.ExecuteFlashCallAsync(
                    initCode, call, flashCallGasLimit, options, requestOptions, cancellationToken);
        }

        int argsLength = DeployedFlashCallEncoding.GetPayloadLength(initCode, call.Data.Length);

        byte[] rented = ArrayPool<byte>.Shared.Rent(argsLength);
        var payload = rented.AsMemory(0, argsLength);

        try
        {
            DeployedFlashCallEncoding.Write(payload.Span, initCode, call.Data.Span, flashCallGasLimit);

            var result = await _ethRpcModule.CallAsync(
                _configuration.ContractAddress,
                null,
                call.Value,
                payload,
                options,
                requestOptions,
                cancellationToken
            );

            return !result.Success
                ? throw CallRevertedException.Parse(_configuration.ContractAddress, result.Data.Span)
                : DeployedFlashCallEncoding.DecodeResult(result.Data);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
