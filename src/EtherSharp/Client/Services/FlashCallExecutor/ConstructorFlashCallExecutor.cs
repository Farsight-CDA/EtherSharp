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
    private static readonly byte[] _flashCallContractUnlimited = Convert.FromHexString(
        "383d3d39602b5160f01c80602d3df03d3d3d84602d018038039034865af181533d8160013e3d60010181f3");

    private static readonly byte[] _flashCallContract = Convert.FromHexString(
        "383d3d396035518060b01c61ffff169060c01c81603f3df0903d3d3d85603f0180380390348787f181533d8160013e3d60010181f3");

    private const int MAX_RUNTIMECODE_SIZE = 24 * 1024;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;

    private ulong ResolveFlashCallGasLimit(ulong flashCallGasLimit)
        => flashCallGasLimit == 0
            ? _callGasLimitSettings.GetFlashCallGasLimit() ?? 0
            : flashCallGasLimit;

    public int GetMaxPayloadSize(ulong flashCallGasLimit, TargetHeight targetHeight)
        => ResolveFlashCallGasLimit(flashCallGasLimit) == 0
            ? EVMByteCode.MAX_INIT_LENGTH - 2 - _flashCallContractUnlimited.Length
            : EVMByteCode.MAX_INIT_LENGTH - 10 - _flashCallContract.Length;

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
        byte[] contract = useUnlimitedPayload ? _flashCallContractUnlimited : _flashCallContract;
        int argsLength = prefixLength + deployment.Data.Length + call.Data.Length;

        if(argsLength + contract.Length > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum call length exceeded, {argsLength + contract.Length} > {EVMByteCode.MAX_INIT_LENGTH}");
        }

        int payloadLength = contract.Length + argsLength;
        byte[] rented = ArrayPool<byte>.Shared.Rent(payloadLength);
        var payload = rented.AsMemory(0, payloadLength);
        contract.CopyTo(payload);
        var buffer = payload.Span[contract.Length..];

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

            if(!result.Success)
            {
                throw CallRevertedException.Parse(null, result.Data.Span);
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
