using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.FlashCall;

internal class ConstructorFlashCallExecutor(IEthRpcModule ethRpcModule) : IFlashCallExecutor
{
    private const string FLASHCALL_CONTRACT_HEX = "0x383d3d39602b5160f01c80602d3df03d3d3d84602d018038039034865af181533d8160013e3d60010181f3";
    private const int FLASHCALL_CONTRACT_LENGTH = 43;

    //EIP-3860
    private const int MAX_INITCODE_SIZE = 48 * 1024;
    private const int MAX_PAYLOAD_SIZE = MAX_INITCODE_SIZE - 2 - FLASHCALL_CONTRACT_LENGTH;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public int GetMaxPayloadSize(TargetBlockNumber targetHeight)
        => MAX_PAYLOAD_SIZE;

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

        Span<byte> buffer = stackalloc byte[argsLength];

        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort) deployment.Data.Length);
        deployment.Data.CopyTo(buffer[2..]);
        call.Data.CopyTo(buffer[(deployment.Data.Length + 2)..]);

        string payload = String.Create(
            FLASHCALL_CONTRACT_HEX.Length + (buffer.Length * 2),
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
}
