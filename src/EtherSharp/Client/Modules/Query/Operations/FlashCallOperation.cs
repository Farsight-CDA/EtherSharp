
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class FlashCallQueryOperation<T>(ReadOnlyMemory<byte> byteCode, ITxInput<T> txInput) : IQuery, IQuery<T>
{
    private readonly ITxInput<T> _txInput = txInput;
    private readonly ReadOnlyMemory<byte> _byteCode = byteCode;

    public int CallDataLength => 1 + 8 + _byteCode.Length + _txInput.Data.Length;
    IReadOnlyList<IQuery> IQuery<T>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.FlashCall;
        buffer = buffer[1..];
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _byteCode.Length);
        BinaryPrimitives.WriteUInt32BigEndian(buffer[4..8], (uint) _txInput.Data.Length);
        _byteCode.Span.CopyTo(buffer[8..(8 + _byteCode.Length)]);
        _txInput.Data.CopyTo(buffer[(8 + _byteCode.Length)..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[1..4].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 4;
    }

    T IQuery<T>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
    {
        var callResults = new TxCallResult[queryResults.Length];

        for(int i = 0; i < queryResults.Length; i++)
        {
            bool success = queryResults[i][0] == 0x01;
            byte[] returnData = queryResults[i][4..];
            callResults[i] = success switch
            {
                true => new TxCallResult.Success(returnData),
                false => new TxCallResult.Reverted(returnData)
            };
        }

        return _txInput.ReadResultFrom(callResults[0].Unwrap(_txInput.To));
    }
}
