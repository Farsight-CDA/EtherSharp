using EtherSharp.ABI.Types;
using EtherSharp.Tx;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class CallAndMeasureGasQueryOperation<T>(IContractCall<T> txInput) : IQuery, IQuery<(QueryResult<T>, ulong)>
{
    private readonly IContractCall<T> _txInput = txInput;

    public int CallDataLength => 4 + 20 + 32 + _txInput.Data.Length;
    public BigInteger EthValue => _txInput.Value;
    IReadOnlyList<IQuery> IQuery<(QueryResult<T>, ulong)>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _txInput.Data.Length);

        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.CallAndMeasureGas;

        _txInput.To.Bytes.CopyTo(buffer[4..24]);
        AbiTypes.BigInteger.EncodeInto(EthValue, true, buffer[24..56]);
        _txInput.Data.Span.CopyTo(buffer[56..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[1..4].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 12;
    }
    (QueryResult<T>, ulong) IQuery<(QueryResult<T>, ulong)>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
    {
        byte[] queryResult = queryResults[0];
        bool success = queryResult[0] == 0x01;
        ulong gasUsed = BinaryPrimitives.ReadUInt64BigEndian(queryResult.AsSpan(4, 8));

        QueryResult<T> result = success switch
        {
            true => new QueryResult<T>.Success(_txInput.ReadResultFrom(queryResult.AsMemory(12))),
            false => new QueryResult<T>.Reverted(queryResult[12..])
        };

        return (result, gasUsed);
    }
}
