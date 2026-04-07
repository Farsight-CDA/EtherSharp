using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCallExecutor;

internal sealed class ConstructorFlashCallExecutor(IEthRpcModule ethRpcModule, CallGasLimitSettings callGasLimitSettings) : IFlashCallExecutor
{
    private const string FLASHCALL_CONTRACT_HEX_UNLIMITED = "0x383d3d39602b5160f01c80602d3df03d3d3d84602d018038039034865af181533d8160013e3d60010181f3";
    private const int FLASHCALL_CONTRACT_LENGTH_UNLIMITED = 43;

    private const string FLASHCALL_CONTRACT_HEX = "0x383d3d396035518060b01c61ffff169060c01c81603f3df0903d3d3d85603f0180380390348787f181533d8160013e3d60010181f3";
    private const int FLASHCALL_CONTRACT_LENGTH = 53;

    //EIP-3860
    private const int MAX_INITCODE_SIZE = 48 * 1024;
    private const int MAX_PAYLOAD_SIZE = MAX_INITCODE_SIZE - 10 - FLASHCALL_CONTRACT_LENGTH;
    private const int MAX_PAYLOAD_SIZE_UNLIMITED = MAX_INITCODE_SIZE - 2 - FLASHCALL_CONTRACT_LENGTH_UNLIMITED;

    private const int MAX_RUNTIMECODE_SIZE = 24 * 1024;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;

    private ulong ResolveFlashCallGasLimit(ulong flashCallGasLimit)
        => flashCallGasLimit == 0
            ? _callGasLimitSettings.GetFlashCallGasLimit() ?? 0
            : flashCallGasLimit;

    public int GetMaxPayloadSize(ulong flashCallGasLimit, TargetHeight targetHeight)
        => ResolveFlashCallGasLimit(flashCallGasLimit) == 0
            ? MAX_PAYLOAD_SIZE_UNLIMITED
            : MAX_PAYLOAD_SIZE;

    public int GetMaxResultSize(TargetHeight targetHeight)
        => MAX_RUNTIMECODE_SIZE;

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

        bool useUnlimitedPayload = flashCallGasLimit == 0;
        int prefixLength = useUnlimitedPayload ? 2 : 10;
        int contractLength = useUnlimitedPayload ? FLASHCALL_CONTRACT_LENGTH_UNLIMITED : FLASHCALL_CONTRACT_LENGTH;
        string contractHex = useUnlimitedPayload ? FLASHCALL_CONTRACT_HEX_UNLIMITED : FLASHCALL_CONTRACT_HEX;
        int argsLength = prefixLength + deployment.Data.Length + call.Data.Length;

        if(argsLength + contractLength > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum call length exceeded, {argsLength + contractLength} > {EVMByteCode.MAX_INIT_LENGTH}");
        }

        byte[]? rented = null;
        var buffer = argsLength <= 4096
            ? stackalloc byte[argsLength]
            : (rented = ArrayPool<byte>.Shared.Rent(argsLength)).AsSpan(0, argsLength);

        try
        {
            if(useUnlimitedPayload)
            {
                BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort) deployment.Data.Length);
                deployment.Data.Span.CopyTo(buffer[2..]);
                call.Data.Span.CopyTo(buffer[(deployment.Data.Length + 2)..]);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer, flashCallGasLimit);
                BinaryPrimitives.WriteUInt16BigEndian(buffer[8..], (ushort) deployment.Data.Length);
                deployment.Data.Span.CopyTo(buffer[10..]);
                call.Data.Span.CopyTo(buffer[(deployment.Data.Length + 10)..]);
            }

            string payload = String.Create(
                contractHex.Length + (argsLength * 2),
                buffer,
                (chars, state) =>
                {
                    contractHex.AsSpan().CopyTo(chars);
                    Convert.TryToHexString(state, chars[contractHex.Length..], out _);
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
