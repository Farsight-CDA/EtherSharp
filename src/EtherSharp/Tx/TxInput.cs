using EtherSharp.ABI;
using EtherSharp.ABI.Decode;

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
    private readonly AbiEncoder _encoder;
    private readonly Func<AbiDecoder, T> _decoder;

    internal TxInput(AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        _encoder = encoder;
        _decoder = decoder;
    }

    public bool TryWriteTo(Span<byte> buffer)
        => _encoder is null || _encoder.TryWritoTo(buffer);

    public T ReadResultFrom(ReadOnlyMemory<byte> buffer) 
        => _decoder.Invoke(new AbiDecoder(buffer));
}

