using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCallExecutor;

internal class ConstructorFlashCallExecutor(IEthRpcModule ethRpcModule) : IFlashCallExecutor
{
    private const string FLASHCALL_CONTRACT_HEX = "0x383d3d39602b5160f01c80602d3df03d3d3d84602d018038039034865af181533d8160013e3d60010181f3";
    private const int FLASHCALL_CONTRACT_LENGTH = 43;

    //EIP-3860
    private const int MAX_INITCODE_SIZE = 48 * 1024;
    private const int MAX_PAYLOAD_SIZE = MAX_INITCODE_SIZE - 2 - FLASHCALL_CONTRACT_LENGTH;

    private const int MAX_RUNTIMECODE_SIZE = 24 * 1024;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public int GetMaxPayloadSize(TargetBlockNumber targetHeight)
        => MAX_PAYLOAD_SIZE;

    public int GetMaxResultSize(TargetBlockNumber targetHeight)
        => MAX_RUNTIMECODE_SIZE;

    public async Task<TxCallResult> ExecuteFlashCallAsync(IContractDeployment deployment, IContractCall call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        if(deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        int argsLength = 2 + deployment.Data.Length + call.Data.Length;

        if(argsLength + FLASHCALL_CONTRACT_LENGTH > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum call length exceeded, {argsLength + FLASHCALL_CONTRACT_LENGTH} > {EVMByteCode.MAX_INIT_LENGTH}");
        }

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
                FLASHCALL_CONTRACT_HEX.Length + (argsLength * 2),
                buffer,
                (chars, buffer) =>
                {
                    FLASHCALL_CONTRACT_HEX.AsSpan().CopyTo(chars);
                    Convert.TryToHexString(buffer, chars[FLASHCALL_CONTRACT_HEX.Length..], out _);
                }
            );

            var result = await _ethRpcModule.CallAsync(
                null,
                null,
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
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}
