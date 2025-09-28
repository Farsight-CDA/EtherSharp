using EtherSharp.ABI;
using EtherSharp.Common.Exceptions;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

internal class TxInput(Address to, BigInteger value, byte[] data)
    : ITxInput
{
    public Address To { get; } = to;

    public BigInteger Value { get; } = value;

    private readonly byte[] _data = data;
    public ReadOnlySpan<byte> Data => _data;
}

internal class TxInput<T>(Address to, BigInteger value, byte[] data, Func<AbiDecoder, T> decoder)
    : TxInput(to, value, data), ITxInput<T>
{
    private readonly Func<AbiDecoder, T> _decoder = decoder;

    public T ReadResultFrom(ReadOnlyMemory<byte> buffer)
    {
        if(buffer.Length == 0)
        {
            throw new CallParsingException.EmptyCallDataException();
        }

        try
        {
            var decoder = new AbiDecoder(buffer);
            var result = _decoder.Invoke(decoder);

            return decoder.BytesRead < buffer.Length
                ? throw new CallParsingException.RemainingCallDataException(buffer.ToArray())
                : result;
        }
        catch(Exception ex)
        {
            throw new CallParsingException.MalformedCallDataException(buffer.ToArray(), ex);
        }
    }
}

