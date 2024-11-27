using EtherSharp.ABI;
using EtherSharp.ABI.Decode;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

public class TxInput : ITxInput
{
    private readonly AbiEncoder? _encoder;

    public Address To { get; }
    public BigInteger Value { get; }

    int ITxInput.DataLength => _encoder?.Size ?? 0;

    internal TxInput(AbiEncoder abiEncoder, Address to, BigInteger value)
    {
        _encoder = abiEncoder;
        To = to;
        Value = value;
    }

    void ITxInput.WriteDataTo(Span<byte> destination)
    {
        if (_encoder is null)
        {
            return;
        }
        if(!_encoder.TryWritoTo(destination))
        {
            throw new InvalidOperationException("Failed to write TxInput");
        }
    }
}

public class TxInput<T> : ITxInput
{
    private readonly ReadOnlyMemory<byte> _functionSignature;
    private readonly AbiEncoder _encoder;
    private readonly Func<AbiDecoder, T> _decoder;

    public Address To { get; }
    public BigInteger Value { get; }

    int ITxInput.DataLength => _functionSignature.Length + _encoder.Size;

    public TxInput(ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder, Address to, BigInteger value)
    {
        To = to;
        _functionSignature = functionSignature;
        _encoder = encoder;
        _decoder = decoder;
        Value = value;
    }

    void ITxInput.WriteDataTo(Span<byte> destination)
    {
        _functionSignature.Span.CopyTo(destination);
        if(!_encoder.TryWritoTo(destination[_functionSignature.Length..]))
        {
            throw new InvalidOperationException("Failed to write TxInput");
        }
    }

    internal T ReadResultFrom(ReadOnlyMemory<byte> buffer)
        => _decoder.Invoke(new AbiDecoder(buffer));
}

