namespace EtherSharp.ABI;
public interface IArrayAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public IArrayAbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public void WriteToParent(Span<byte> result, Span<byte> payload, uint payloadOffset);

    public IArrayAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal void Build(Span<byte> result);
}
