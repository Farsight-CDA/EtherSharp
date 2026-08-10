using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;
using System.Numerics;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class ConstructorFlashCallExecutor(IEthRpcModule ethRpcModule) : IFlashInitCodeExecutor
{
    private const int FIXED_HELPER_LENGTH = 27;
    private const int MAX_UNLIMITED_HELPER_LENGTH = 37;
    private const int MAX_LIMITED_HELPER_LENGTH = 45;
    private const int MAX_RUNTIMECODE_SIZE = 24 * 1024;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public int GetMaxPayloadSize(int initCodeLength, ulong? flashCallGasLimit, TargetHeight targetHeight)
        => (flashCallGasLimit is null
                ? EVMByteCode.MAX_INIT_LENGTH - MAX_UNLIMITED_HELPER_LENGTH
                : EVMByteCode.MAX_INIT_LENGTH - MAX_LIMITED_HELPER_LENGTH
            ) - initCodeLength;

    public int GetMaxResultSize(TargetHeight targetHeight)
        => MAX_RUNTIMECODE_SIZE;

    public async Task<TxCallResult> ExecuteFlashCallAsync(
        EVMByteCode initCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        CancellationToken cancellationToken)
    {
        int helperLength = GetHelperLength(initCode.Length, call.Data.Length, flashCallGasLimit);
        int argsLength = initCode.Length + call.Data.Length;

        if(argsLength + helperLength > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum call length exceeded, {argsLength + helperLength} > {EVMByteCode.MAX_INIT_LENGTH}");
        }

        int payloadLength = helperLength + argsLength;
        byte[] rented = ArrayPool<byte>.Shared.Rent(payloadLength);
        var payload = rented.AsMemory(0, payloadLength);

        try
        {
            WriteHelper(payload.Span, helperLength, initCode.Length, call.Data.Length, flashCallGasLimit);
            initCode.ByteCode.Span.CopyTo(payload.Span[helperLength..]);
            call.Data.Span.CopyTo(payload.Span[(helperLength + initCode.Length)..]);

            var result = await _ethRpcModule.CallAsync(
                null,
                null,
                null,
                call.Value,
                payload,
                options,
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

        destination[offset++] = 0x38; // CODESIZE
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x39; // CODECOPY
        offset = WritePush(destination, offset, (ulong) deploymentLength);
        destination[offset++] = 0x60; // PUSH1
        destination[offset++] = (byte) helperLength;
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0xF0; // CREATE
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x3D; // RETURNDATASIZE
        offset = WritePush(destination, offset, (ulong) callLength);
        offset = WritePush(destination, offset, (ulong) (helperLength + deploymentLength));
        destination[offset++] = 0x34; // CALLVALUE
        destination[offset++] = 0x86; // DUP7

        if(flashCallGasLimit is null)
        {
            destination[offset++] = 0x5A; // GAS
        }
        else
        {
            offset = WritePush(destination, offset, flashCallGasLimit.Value);
        }

        destination[offset++] = 0xF1; // CALL
        destination[offset++] = 0x81; // DUP2
        destination[offset++] = 0x53; // MSTORE8
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x81; // DUP2
        destination[offset++] = 0x60; // PUSH1
        destination[offset++] = 0x01;
        destination[offset++] = 0x3E; // RETURNDATACOPY
        destination[offset++] = 0x3D; // RETURNDATASIZE
        destination[offset++] = 0x60; // PUSH1
        destination[offset++] = 0x01;
        destination[offset++] = 0x01; // ADD
        destination[offset++] = 0x81; // DUP2
        destination[offset++] = 0xF3; // RETURN
    }

    private static int WritePush(Span<byte> destination, int offset, ulong value)
    {
        if(value == 0)
        {
            destination[offset] = 0x3D; // RETURNDATASIZE
            return offset + 1;
        }

        int valueLength = GetPushInstructionLength(value) - 1;
        destination[offset] = (byte) (0x5F + valueLength);

        for(int i = valueLength; i > 0; i--)
        {
            destination[offset + i] = (byte) value;
            value >>= 8;
        }

        return offset + valueLength + 1;
    }
}
