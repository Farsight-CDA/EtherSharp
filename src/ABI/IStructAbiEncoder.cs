namespace EtherSharp.ABI;
public interface IStructAbiEncoder : IAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public new IStructAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal void Build(Span<byte> result);
}
