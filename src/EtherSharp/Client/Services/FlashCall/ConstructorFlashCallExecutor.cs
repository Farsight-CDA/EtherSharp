using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Numerics;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class ConstructorFlashCallExecutor(
    IEthRpcModule ethRpcModule,
    CallGasLimitSettings callGasLimitSettings) : FlashCallExecutor
{
    private const int FIXED_HELPER_LENGTH = 38;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;

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

        int initCodeLength = EVMInitCode.GetLengthFromRuntimeCode(runtimeCode);
        int helperLength = GetHelperLength(initCodeLength, call.Data.Length, flashCallGasLimit);
        int argsLength = initCodeLength + call.Data.Length;

        int payloadLength = helperLength + argsLength;
        byte[] rented = ArrayPool<byte>.Shared.Rent(payloadLength);
        var payload = rented.AsMemory(0, payloadLength);

        try
        {
            WriteHelper(payload.Span, helperLength, initCodeLength, call.Data.Length, flashCallGasLimit);
            EVMInitCode.WriteFromRuntimeCode(runtimeCode, payload.Span[helperLength..]);
            call.Data.Span.CopyTo(payload.Span[(helperLength + initCodeLength)..]);

            var result = await _ethRpcModule.CallAsync(
                null,
                null,
                call.Value,
                payload,
                options,
                requestOptions,
                cancellationToken
            );

            if(!result.Success)
            {
                throw CallRevertedException.Parse(null, result.Data.Span);
            }

            var data = result.Data;

            return data.IsEmpty
                ? throw new CallParsingException.EmptyReturnDataException()
                : data.Span[0] switch
                {
                    0 => new TxCallResult(false, data[1..]),
                    1 => new TxCallResult(true, data[1..]),
                    _ => throw new CallParsingException.MalformedReturnDataException(
                        data, $"The constructor flash caller returned an invalid status byte: {data.Span[0]}."
                    )
                };
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private static int GetHelperLength(int deploymentLength, int callLength, ulong? flashCallGasLimit)
    {
        int gasInstructionLength = flashCallGasLimit is null
            ? 1
            : GetPushInstructionLength(flashCallGasLimit.Value);
        int helperLength = FIXED_HELPER_LENGTH
            + GetPushInstructionLength((ulong) deploymentLength)
            + GetPushInstructionLength((ulong) callLength)
            + 2
            + gasInstructionLength;

        return helperLength + GetPushInstructionLength((ulong) (helperLength + deploymentLength)) - 2;
    }

    private static int GetPushInstructionLength(ulong value)
        => value == 0
            ? 1
            : (BitOperations.Log2(value) / 8) + 2;

    private static void WriteHelper(
        Span<byte> destination,
        int helperLength,
        int deploymentLength,
        int callLength,
        ulong? flashCallGasLimit)
    {
        int offset = 0;

        destination[offset++] = (byte) EvmOpcode.CodeSize;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.CodeCopy;
        offset = WritePush(destination, offset, (ulong) deploymentLength);
        destination[offset++] = (byte) EvmOpcode.Push1;
        destination[offset++] = (byte) helperLength;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.Create;

        // A failed CREATE leaves its revert data in the return-data buffer.
        // Skip CALL and encode that failure using the common result writer.
        destination[offset++] = (byte) EvmOpcode.Dup1;
        destination[offset++] = (byte) EvmOpcode.Push1;
        int callTargetOffset = offset++;
        destination[offset++] = (byte) EvmOpcode.JumpI;
        destination[offset++] = (byte) EvmOpcode.Dup1;
        destination[offset++] = (byte) EvmOpcode.Dup1;
        destination[offset++] = (byte) EvmOpcode.Push1;
        int resultTargetOffset = offset++;
        destination[offset++] = (byte) EvmOpcode.Jump;
        destination[callTargetOffset] = (byte) offset;
        destination[offset++] = (byte) EvmOpcode.JumpDest;

        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        offset = WritePush(destination, offset, (ulong) callLength);
        offset = WritePush(destination, offset, (ulong) (helperLength + deploymentLength));
        destination[offset++] = (byte) EvmOpcode.CallValue;
        destination[offset++] = (byte) EvmOpcode.Dup7;

        if(flashCallGasLimit is null)
        {
            destination[offset++] = (byte) EvmOpcode.Gas;
        }
        else
        {
            offset = WritePush(destination, offset, flashCallGasLimit.Value);
        }

        destination[offset++] = (byte) EvmOpcode.Call;
        destination[resultTargetOffset] = (byte) offset;
        destination[offset++] = (byte) EvmOpcode.JumpDest;
        destination[offset++] = (byte) EvmOpcode.Dup2;
        destination[offset++] = (byte) EvmOpcode.MStore8;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.Dup2;
        destination[offset++] = (byte) EvmOpcode.Push1;
        destination[offset++] = 0x01;
        destination[offset++] = (byte) EvmOpcode.ReturnDataCopy;
        destination[offset++] = (byte) EvmOpcode.ReturnDataSize;
        destination[offset++] = (byte) EvmOpcode.Push1;
        destination[offset++] = 0x01;
        destination[offset++] = (byte) EvmOpcode.Add;
        destination[offset++] = (byte) EvmOpcode.Dup2;
        destination[offset++] = (byte) EvmOpcode.Return;
    }

    private static int WritePush(Span<byte> destination, int offset, ulong value)
    {
        if(value == 0)
        {
            destination[offset] = (byte) EvmOpcode.ReturnDataSize;
            return offset + 1;
        }

        int valueLength = GetPushInstructionLength(value) - 1;
        destination[offset] = (byte) ((byte) EvmOpcode.Push0 + valueLength);

        for(int i = valueLength; i > 0; i--)
        {
            destination[offset + i] = (byte) value;
            value >>= 8;
        }

        return offset + valueLength + 1;
    }
}
