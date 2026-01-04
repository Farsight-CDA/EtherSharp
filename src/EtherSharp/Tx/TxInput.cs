using EtherSharp.Common.Exceptions;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

internal class TxInput(Address to, BigInteger value, byte[] data)
    : IContractCall, ITxInput
{
    public Address To { get; } = to;

    public BigInteger Value { get; } = value;

    private readonly byte[] _data = data;
    public ReadOnlySpan<byte> Data => _data;
}

internal class TxInput<T>(Address to, BigInteger value, byte[] data, Func<ReadOnlyMemory<byte>, T> decoder) : TxInput(to, value, data), IContractCall<T>, ITxInput<T>
{
    private readonly Func<ReadOnlyMemory<byte>, T> _decoder = decoder;

    public T ReadResultFrom(ReadOnlyMemory<byte> data)
    {
        if(data.Length == 0)
        {
            throw new CallParsingException.EmptyCallDataException();
        }

        try
        {
            var result = _decoder.Invoke(data);
            //ToDo: Check for remaining data
            return result;
        }
        catch(Exception ex)
        {
            throw new CallParsingException.MalformedCallDataException(data, ex);
        }
    }
}

internal record RawTxInput(Address To, BigInteger Value, ReadOnlyMemory<byte> Data) : IContractCall<ReadOnlyMemory<byte>>, ITxInput<ReadOnlyMemory<byte>>
{
    ReadOnlySpan<byte> ITxInput.Data => Data.Span;
    public ReadOnlyMemory<byte> ReadResultFrom(ReadOnlyMemory<byte> data) => data;
}

