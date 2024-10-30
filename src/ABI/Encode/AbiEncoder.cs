using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Encode;
using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI;

public partial class AbiDecoder : IAbiEncoder, IArrayAbiEncoder, IStructAbiEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private uint _payloadSize = 0;
    private uint _metadataSize = 0;

    public uint Size => _payloadSize + _metadataSize;
    uint IArrayAbiEncoder.MetadataSize => _metadataSize;
    uint IArrayAbiEncoder.PayloadSize => _payloadSize;
    uint IStructAbiEncoder.MetadataSize => _metadataSize;
    uint IStructAbiEncoder.PayloadSize => _payloadSize;

    private AbiDecoder AddElement(IEncodeType item)
    {
        if(item is IDynamicType dyn)
        {
            _payloadSize += dyn.PayloadSize;
        }

        _metadataSize += 32;
        _entries.Add(item);
        return this;
    }

    public AbiDecoder UInt8(byte value)
        => AddElement(new FixedType<string>.Byte(value));
    public AbiDecoder Int8(sbyte value)
        => AddElement(new FixedType<string>.SByte(value));
    public AbiDecoder UInt16(ushort value)
        => AddElement(new FixedType<string>.UShort(value));
    public AbiDecoder Int16(short value)
        => AddElement(new FixedType<string>.Short(value));

    public AbiDecoder String(string value)
        => AddElement(new DynamicType<string>.String(value));
    public AbiDecoder Bytes(byte[] arr)
        => AddElement(new DynamicType<string>.Bytes(arr));

    public AbiDecoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
        => AddElement(new DynamicType<string>.Array(func(new AbiDecoder())));
    public AbiDecoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => AddElement(new DynamicType<string>.Struct(typeId, func(new AbiDecoder())));

    IArrayAbiEncoder IArrayAbiEncoder.Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
        => Array(func);
    IArrayAbiEncoder IArrayAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => Struct(typeId, func);
    IStructAbiEncoder IStructAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => Struct(typeId, func);

    public void Build(Span<byte> result)
    {
        uint metadataOffset = 0;
        uint payloadOffset = _metadataSize;

        foreach(var entry in _entries)
        {
            switch(entry)
            {
                case IDynamicType dynEncType:
                    dynEncType.Encode(
                        result.Slice(
                            (int) metadataOffset,
                            32),
                        result.Slice(
                            (int) payloadOffset,
                            (int) dynEncType.PayloadSize),
                        payloadOffset
                    );
                    payloadOffset += dynEncType.PayloadSize;
                    break;
                case IFixedType fixEncType:
                    fixEncType.Encode(
                        result.Slice(
                            (int) metadataOffset,
                            32)
                    );
                    break;
                default:
                    throw new NotImplementedException(entry.GetType().FullName);
            }

            metadataOffset += 32;
        }

        _entries.Clear();
    }
}