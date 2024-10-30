namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IStructAbiEncoder : IAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public new IStructAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal void WritoTo(Span<byte> result);
}
