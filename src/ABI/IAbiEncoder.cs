namespace EtherSharp.ABI;
public partial interface IAbiEncoder
{
    public int Size { get; }

    public AbiEncoder UInt8(byte value);

    public AbiEncoder Int8(sbyte value);

    public AbiEncoder UInt16(ushort value);

    public AbiEncoder Int16(short value);

    public AbiEncoder String(string value);

    public AbiEncoder Bytes(byte[] arr);

    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public AbiEncoder Struct(Func<IStructAbiEncoder, IStructAbiEncoder> func);
}