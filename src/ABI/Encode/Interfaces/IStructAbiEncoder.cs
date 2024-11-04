namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IStructAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public AbiEncoder UInt8(byte value);

    public AbiEncoder Int8(sbyte value);

    public AbiEncoder UInt16(ushort value);

    public AbiEncoder Int16(short value);

    public AbiEncoder String(string value);

    public AbiEncoder Bytes(byte[] arr);

    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public IStructAbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);

    internal void WritoTo(Span<byte> result);
}
