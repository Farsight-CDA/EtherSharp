namespace EtherSharp.ABI.Decode;
public partial class ArrayAbiDecoder(Memory<byte> bytes)
{
    private readonly Memory<byte> _bytes = bytes;

    private Span<byte> Bytes => _bytes.Span[(int) _currentMetadataIndex..];

    private uint _currentMetadataIndex = 0;

    private ArrayAbiDecoder ConsumeBytes(uint payloadSize)
    {
        _currentMetadataIndex += 32;
        return this;
    }

    public T Struct<T>(Func<StructAbiDecoder, T> func) => throw new NotImplementedException();
    public T[] Array<T>(Func<StructAbiDecoder, T> func) => throw new NotImplementedException();

}
