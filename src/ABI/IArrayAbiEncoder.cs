namespace EtherSharp.ABI;
public interface IArrayAbiEncoder
{
    public IArrayAbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public void Build(Span<byte> result, Span<byte> payload, int payloadOffset);

    public int MetadataSize { get; }

    public int PayloadSize { get; }
}
