using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCallExecutor;

internal record DeployedFlashCallExecutorConfiguration(Address ContractAddress, bool AllowFallback, int MaxPayloadSize, int MaxResultSize);
internal class DeployedFlashCallExecutor(IEthRpcModule ethRpcModule, DeployedFlashCallExecutorConfiguration configuration) : IFlashCallExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly DeployedFlashCallExecutorConfiguration _configuration = configuration;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule);

    private ulong _deploymentHeight;

    public Address ContractAddress => _configuration.ContractAddress;

    public void SetDeploymentHeight(ulong deploymentHeight)
        => _deploymentHeight = deploymentHeight;

    public int GetMaxPayloadSize(TargetHeight targetHeight)
    {
        bool useFallback = targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight;
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxPayloadSize(targetHeight)
            : _configuration.MaxPayloadSize;
    }
    public int GetMaxResultSize(TargetHeight targetHeight)
    {
        bool useFallback = targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight;
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxResultSize(targetHeight)
            : _configuration.MaxResultSize;
    }

    public async Task<TxCallResult> ExecuteFlashCallAsync(IContractDeployment deployment, IContractCall call, TargetHeight targetHeight, CancellationToken cancellationToken)
    {
        if(deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        if(_deploymentHeight == 0)
        {
            return await _constructorFlashCallExecutor.ExecuteFlashCallAsync(deployment, call, targetHeight, cancellationToken);
        }

        if(targetHeight.Value > 0 && targetHeight.Value <= _deploymentHeight)
        {
            return !_configuration.AllowFallback
                ? throw new InvalidOperationException($"Missing FlashCall contract deployment at height {targetHeight.Value}")
                : await _constructorFlashCallExecutor.ExecuteFlashCallAsync(deployment, call, targetHeight, cancellationToken);
        }

        int argsLength = 2 + deployment.Data.Length + call.Data.Length;

        byte[]? rented = null;
        var buffer = argsLength <= 4096
            ? stackalloc byte[argsLength]
            : (rented = ArrayPool<byte>.Shared.Rent(argsLength)).AsSpan(0, argsLength);

        try
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort) deployment.Data.Length);
            deployment.Data.Span.CopyTo(buffer[2..]);
            call.Data.Span.CopyTo(buffer[(deployment.Data.Length + 2)..]);

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
