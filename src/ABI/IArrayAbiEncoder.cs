namespace EtherSharp.ABI;
public interface IArrayAbiEncoder
{
    public int MetadataSize { get; }

    public int PayloadSize { get; }

    public IArrayAbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public void WriteToParent(Span<byte> result, Span<byte> payload, int payloadOffset);
}
