using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;

internal class TxInput(Address to, UInt256 value, ReadOnlyMemory<byte> data)
    : IContractCall, ITxInput
{
    public Address To { get; } = to;

    public UInt256 Value { get; } = value;
    public ReadOnlyMemory<byte> Data { get; } = data;
}

internal class TxInput<T>(Address to, UInt256 value, ReadOnlyMemory<byte> data, Func<ReadOnlyMemory<byte>, T> decoder)
    : TxInput(to, value, data), IContractCall<T>, ITxInput<T>
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
