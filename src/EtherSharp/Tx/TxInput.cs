using EtherSharp.ABI;
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

internal class TxInput<T>(Address to, BigInteger value, byte[] data, Func<AbiDecoder, T> decoder)
    : TxInput(to, value, data), IContractCall<T>, ITxInput<T>
{
    private readonly Func<AbiDecoder, T> _decoder = decoder;

    public T ReadResultFrom(ReadOnlyMemory<byte> data)
    {
        if(data.Length == 0)
        {
            throw new CallParsingException.EmptyCallDataException();
        }

        try
        {
            var decoder = new AbiDecoder(data);
            var result = _decoder.Invoke(decoder);
            //ToDo: Check for remaining data
            return result;
        }
        catch(Exception ex)
        {
            throw new CallParsingException.MalformedCallDataException(data, ex);
        }
    }
}

