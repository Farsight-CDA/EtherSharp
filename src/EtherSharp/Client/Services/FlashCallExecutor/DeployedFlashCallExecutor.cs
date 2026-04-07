using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCallExecutor;

internal sealed record DeployedFlashCallExecutorConfiguration(Address ContractAddress, bool AllowFallback, int MaxPayloadSize, int MaxResultSize);
internal sealed class DeployedFlashCallExecutor(IEthRpcModule ethRpcModule, DeployedFlashCallExecutorConfiguration configuration,
    CallGasLimitSettings callGasLimitSettings) : IFlashCallExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly DeployedFlashCallExecutorConfiguration _configuration = configuration;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule, callGasLimitSettings);

    private ulong _deploymentHeight;

    private ulong ResolveFlashCallGasLimit(ulong flashCallGasLimit)
        => flashCallGasLimit == 0
            ? _callGasLimitSettings.GetFlashCallGasLimit() ?? 0
            : flashCallGasLimit;

    public Address ContractAddress => _configuration.ContractAddress;

    public void SetDeploymentHeight(ulong deploymentHeight)
        => _deploymentHeight = deploymentHeight;

    public int GetMaxPayloadSize(ulong flashCallGasLimit, TargetHeight targetHeight)
    {
        bool useFallback = targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight;
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxPayloadSize(flashCallGasLimit, targetHeight)
            : _configuration.MaxPayloadSize - 10;
    }
    public int GetMaxResultSize(TargetHeight targetHeight)
    {
        bool useFallback = targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight;
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxResultSize(targetHeight)
            : _configuration.MaxResultSize;
    }

    public async Task<TxCallResult> ExecuteFlashCallAsync(
        IContractDeployment deployment,
        IFlashCall call,
        ulong flashCallGasLimit,
        TargetHeight targetHeight,
        CancellationToken cancellationToken)
    {
        flashCallGasLimit = ResolveFlashCallGasLimit(flashCallGasLimit);

        if(deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        if(_deploymentHeight == 0)
        {
            return await _constructorFlashCallExecutor.ExecuteFlashCallAsync(deployment, call, flashCallGasLimit, targetHeight, cancellationToken);
        }

        if(targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight)
        {
            return !_configuration.AllowFallback
                ? throw new InvalidOperationException($"Missing FlashCall contract deployment at height {targetHeight.Value}")
                : await _constructorFlashCallExecutor.ExecuteFlashCallAsync(deployment, call, flashCallGasLimit, targetHeight, cancellationToken);
        }

        int argsLength = 10 + deployment.Data.Length + call.Data.Length;

        byte[]? rented = null;
        var buffer = argsLength <= 4096
            ? stackalloc byte[argsLength]
            : (rented = ArrayPool<byte>.Shared.Rent(argsLength)).AsSpan(0, argsLength);

        try
        {
            BinaryPrimitives.WriteUInt64BigEndian(buffer, flashCallGasLimit);
            BinaryPrimitives.WriteUInt16BigEndian(buffer[8..], (ushort) deployment.Data.Length);
            deployment.Data.Span.CopyTo(buffer[10..]);
            call.Data.Span.CopyTo(buffer[(deployment.Data.Length + 10)..]);

            string payload = String.Create(
                2 + (argsLength * 2),
                buffer,
                (chars, buffer) =>
                {
                    "0x".CopyTo(chars);
                    Convert.TryToHexString(buffer, chars[2..], out _);
                }
            );

            var result = await _ethRpcModule.CallAsync(
                null,
                _configuration.ContractAddress,
                null,
                null,
                call.Value,
                payload,
                targetHeight,
                cancellationToken
            );

            var data = result.Unwrap(_configuration.ContractAddress);

            return data.Span[0] switch
            {
                0 => new TxCallResult.Reverted(data[1..]),
                1 => new TxCallResult.Success(data[1..]),
                _ => throw new ImpossibleException()
            };
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}
