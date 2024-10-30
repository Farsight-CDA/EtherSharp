using EtherSharp.ABI.Encode;

namespace EtherSharp.ABI;
public partial interface IAbiEncoder
{
    public AbiDecoder UInt8(byte value);

    public AbiDecoder Int8(sbyte value);

    public AbiDecoder UInt16(ushort value);

    public AbiDecoder Int16(short value);

    public AbiDecoder String(string value);

    public AbiDecoder Bytes(byte[] arr);

    public AbiDecoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func);

    public AbiDecoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func);
}