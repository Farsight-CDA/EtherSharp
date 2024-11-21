using EtherSharp.ABI;
using EtherSharp.ABI.Decode;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;

public class TxInput
{
    private readonly AbiEncoder? _encoder;

    public int Length => _encoder?.Size ?? 0;

    internal TxInput(AbiEncoder abiEncoder)
    {
        _encoder = abiEncoder;
    }
    internal TxInput()
    {
    }

    public bool TryWriteTo(Span<byte> buffer)
        => _encoder is null || _encoder.TryWritoTo(buffer);
}

public class TxInput<T>
{
    private readonly ReadOnlyMemory<byte> _functionSignature;
    private readonly AbiEncoder _encoder;
    private readonly Func<AbiDecoder, T> _decoder;

    public Address Target { get; }
    public BigInteger Value { get; }

    internal int DataLength => _functionSignature.Length + _encoder.Size;

    internal TxInput(Address target, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder, BigInteger value)
    {
        Target = target;
        _functionSignature = functionSignature;
        _encoder = encoder;
        _decoder = decoder;
        Value = value;
    }

    internal void WriteDataTo(Span<byte> destination)
    {
        _functionSignature.Span.CopyTo(destination);
        _ = _encoder.TryWritoTo(destination[_functionSignature.Length..]);
    }

    internal T ReadResultFrom(ReadOnlyMemory<byte> buffer)
        => _decoder.Invoke(new AbiDecoder(buffer));
}

