using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCallExecutor;

internal record DeployedFlashCallExecutorConfiguration(ulong DeploymentHeight, Address ContractAddress, bool AllowFallback, int MaxPayloadSize);
internal class DeployedFlashCallExecutor(IEthRpcModule ethRpcModule, DeployedFlashCallExecutorConfiguration configuration) : IFlashCallExecutor
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly DeployedFlashCallExecutorConfiguration _configuration = configuration;
    private readonly ConstructorFlashCallExecutor _constructorFlashCallExecutor = new ConstructorFlashCallExecutor(ethRpcModule);

    //Arbitrary limit
    public int MaxPayloadSize => _configuration.MaxPayloadSize;

    public int GetMaxPayloadSize(TargetBlockNumber targetHeight)
    {
        bool useFallback = targetHeight.Value > 0 && targetHeight.Value <= _configuration.DeploymentHeight;
        return useFallback
            ? _constructorFlashCallExecutor.GetMaxPayloadSize(targetHeight)
            : _configuration.MaxPayloadSize;
    }

    public async Task<TxCallResult> ExecuteFlashCallAsync(IContractDeployment deployment, IContractCall call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        if(deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        if(targetHeight.Value > 0 && targetHeight.Value <= _configuration.DeploymentHeight)
        {
            return !_configuration.AllowFallback
                ? throw new InvalidOperationException($"Missing FlashCall contract deployment at height {targetHeight.Value}")
                : await _constructorFlashCallExecutor.ExecuteFlashCallAsync(deployment, call, targetHeight, cancellationToken);
        }

        int argsLength = 2 + deployment.Data.Length + call.Data.Length;
        Span<byte> buffer = stackalloc byte[argsLength];

        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort) deployment.Data.Length);
        deployment.Data.Span.CopyTo(buffer[2..]);
        call.Data.Span.CopyTo(buffer[(deployment.Data.Length + 2)..]);

        string payload = String.Create(
            2 + (buffer.Length * 2),
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

        var data = result.Unwrap(null);

        return data.Span[0] switch
        {
            0 => new TxCallResult.Reverted(data[1..]),
            1 => new TxCallResult.Success(data[1..]),
            _ => throw new ImpossibleException()
        };
    }
}
