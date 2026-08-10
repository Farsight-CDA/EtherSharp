using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class DeployedFlashCallExecutor(
    IEthRpcModule ethRpcModule,
    DeployedFlashCallExecutor.Configuration configuration) : IFlashInitCodeExecutor
{
    internal sealed record Configuration(
        Address ContractAddress,
        bool AllowFallback,
        int MaxPayloadSize,
        int MaxResultSize
    );

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly Configuration _configuration = configuration;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule);

    private ulong? _deploymentHeight;

    public Address ContractAddress => _configuration.ContractAddress;

    public void SetDeploymentHeight(ulong deploymentHeight)
        => _deploymentHeight = deploymentHeight;

    public int GetMaxPayloadSize(int initCodeLength, ulong? flashCallGasLimit, TargetHeight targetHeight)
    {
        bool useFallback = _deploymentHeight is null
            || (targetHeight.Value > 0 && targetHeight.Value < _deploymentHeight.Value);
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxPayloadSize(initCodeLength, flashCallGasLimit, targetHeight)
            : _configuration.MaxPayloadSize - 10 - initCodeLength;
    }
    public int GetMaxResultSize(TargetHeight targetHeight)
    {
        bool useFallback = _deploymentHeight is null
            || (targetHeight.Value > 0 && targetHeight.Value < _deploymentHeight.Value);
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxResultSize(targetHeight)
            : _configuration.MaxResultSize;
    }

    public async Task<TxCallResult> ExecuteFlashCallAsync(
        EVMByteCode initCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        CancellationToken cancellationToken)
    {
        var targetHeight = options.TargetHeight;

        if(_deploymentHeight is null)
        {
            return await _constructorFlashCallExecutor.ExecuteFlashCallAsync(initCode, call, flashCallGasLimit, options, cancellationToken);
        }

        if(targetHeight.Value > 0 && targetHeight.Value < _deploymentHeight.Value)
        {
            return !_configuration.AllowFallback
                ? throw new InvalidOperationException($"Missing FlashCall contract deployment at height {targetHeight.Value}")
                : await _constructorFlashCallExecutor.ExecuteFlashCallAsync(initCode, call, flashCallGasLimit, options, cancellationToken);
        }

        int argsLength = 10 + initCode.Length + call.Data.Length;

        byte[] rented = ArrayPool<byte>.Shared.Rent(argsLength);
        var payload = rented.AsMemory(0, argsLength);

        try
        {
            BinaryPrimitives.WriteUInt64BigEndian(payload.Span, flashCallGasLimit ?? 0);
            BinaryPrimitives.WriteUInt16BigEndian(payload.Span[8..], (ushort) initCode.Length);
            initCode.ByteCode.Span.CopyTo(payload.Span[10..]);
            call.Data.Span.CopyTo(payload.Span[(initCode.Length + 10)..]);

            var result = await _ethRpcModule.CallAsync(
                _configuration.ContractAddress,
                null,
                null,
                call.Value,
                payload,
                options,
                cancellationToken
            );

            if(!result.Success)
            {
                throw CallRevertedException.Parse(_configuration.ContractAddress, result.Data.Span);
            }

            var data = result.Data;

            return data.Span[0] switch
            {
                0 => new TxCallResult(false, data[1..]),
                1 => new TxCallResult(true, data[1..]),
                _ => throw new ImpossibleException()
            };
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
