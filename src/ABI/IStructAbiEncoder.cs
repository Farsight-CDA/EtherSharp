namespace EtherSharp.ABI;
public interface IStructAbiEncoder : IAbiEncoder
{
    public int MetadataSize { get; }

    public int PayloadSize { get; }

    public new IStructAbiEncoder Struct(Func<IStructAbiEncoder, IStructAbiEncoder> func);

    public new void WriteToParent(Span<byte> result, Span<byte> payload, int payloadOffset);
}
