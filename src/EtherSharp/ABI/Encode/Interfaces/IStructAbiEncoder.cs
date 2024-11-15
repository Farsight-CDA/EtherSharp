namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IStructAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public AbiEncoder String(string value);
    public AbiEncoder Bytes(byte[] arr);
    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);
    public IStructAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
