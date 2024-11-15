namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IArrayAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public IArrayAbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);
    public IArrayAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
