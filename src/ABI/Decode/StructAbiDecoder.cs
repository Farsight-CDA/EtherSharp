namespace EtherSharp.ABI.Decode;
public class StructAbiDecoder(Memory<byte> bytes)
{

    private readonly Memory<byte> _bytes = bytes;

    private Span<byte> Bytes => _bytes.Span[(int) _currentMetadataIndex..];

    private uint _currentMetadataIndex = 0;

    private StructAbiDecoder ConsumeBytes(uint payloadSize)
    {
        _currentMetadataIndex += 32;
        return this;
    }

    public uint MetadataIndex => throw new NotImplementedException();

    public uint PayloadIndex => throw new NotImplementedException();

    public short Int16() => throw new NotImplementedException();
    public sbyte Int8() => throw new NotImplementedException();
    public T Struct<T>(Func<StructAbiDecoder, T> func) => throw new NotImplementedException();
    public ushort UInt16() => throw new NotImplementedException();
    public byte UInt8() => throw new NotImplementedException();
}
