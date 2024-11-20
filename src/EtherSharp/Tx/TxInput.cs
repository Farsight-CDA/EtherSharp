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
    private readonly ReadOnlyMemory<byte> _functionSignature;
    private readonly AbiEncoder _encoder;
    private readonly Func<AbiDecoder, T> _decoder;

    public string Target { get; }

    internal TxInput(string target, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        Target = target;
        _functionSignature = functionSignature;
        _encoder = encoder;
        _decoder = decoder;
    }

    internal string GetCalldataHex()
    {
        Span<byte> buffer = stackalloc byte[_functionSignature.Length + _encoder.Size];

        _functionSignature.Span.CopyTo(buffer);
        _encoder.TryWritoTo(buffer[_functionSignature.Length..]);

        return $"0x{Convert.ToHexString(buffer)}";
    }

    internal T ReadResultFrom(ReadOnlyMemory<byte> buffer)
        => _decoder.Invoke(new AbiDecoder(buffer));
}

