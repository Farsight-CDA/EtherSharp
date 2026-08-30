using EtherSharp.Contract;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Services.FlashCall;

internal static class DeployedFlashCallEncoding
{
    private const int HEADER_LENGTH = sizeof(ulong) + sizeof(ushort);

    public static byte[] Encode(EVMByteCode initCode, ReadOnlyMemory<byte> callData, ulong? flashCallGasLimit)
    {
        byte[] payload = new byte[GetPayloadLength(initCode, callData.Length)];
        Write(payload, initCode, callData.Span, flashCallGasLimit);
        return payload;
    }

    public static int GetPayloadLength(EVMByteCode initCode, int callDataLength)
        => initCode.Length > UInt16.MaxValue
            ? throw new InvalidOperationException($"Deployed flash calls cannot encode initcode longer than {UInt16.MaxValue} bytes.")
            : checked(HEADER_LENGTH + initCode.Length + callDataLength);

    public static void Write(Span<byte> destination, EVMByteCode initCode, ReadOnlySpan<byte> callData, ulong? flashCallGasLimit)
    {
        int payloadLength = GetPayloadLength(initCode, callData.Length);

        if(destination.Length < payloadLength)
        {
            throw new ArgumentException("Destination is too short for the encoded flash call.", nameof(destination));
        }

        BinaryPrimitives.WriteUInt64BigEndian(destination, flashCallGasLimit ?? 0);
        BinaryPrimitives.WriteUInt16BigEndian(destination[sizeof(ulong)..], (ushort) initCode.Length);
        initCode.ByteCode.Span.CopyTo(destination[HEADER_LENGTH..]);
        callData.CopyTo(destination[(HEADER_LENGTH + initCode.Length)..]);
    }

    public static TxCallResult DecodeResult(ReadOnlyMemory<byte> data)
    {
        if(data.IsEmpty)
        {
            throw new InvalidDataException("The deployed flash caller returned no status byte.");
        }
        //
        return data.Span[0] switch
        {
            0 => new TxCallResult(false, data[1..]),
            1 => new TxCallResult(true, data[1..]),
            _ => throw new InvalidDataException($"The deployed flash caller returned an invalid status byte: {data.Span[0]}.")
        };
    }
}
