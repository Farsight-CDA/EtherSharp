using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI;

public partial class AbiEncoder : IAbiEncoder, IArrayAbiEncoder, IStructAbiEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private uint _payloadSize = 0;
    private uint _metadataSize = 0;

    public uint Size => _payloadSize + _metadataSize;
    uint IArrayAbiEncoder.MetadataSize => _metadataSize;
    uint IArrayAbiEncoder.PayloadSize => _payloadSize;
    uint IStructAbiEncoder.MetadataSize => _metadataSize;
    uint IStructAbiEncoder.PayloadSize => _payloadSize;

    private AbiEncoder AddElement(IEncodeType item)
    {
        if(item is IDynamicEncodeType dyn)
        {
            _payloadSize += dyn.PayloadSize;
        }

        _metadataSize += 32;
        _entries.Add(item);
        return this;
    }

    public AbiEncoder UInt8(byte value) 
        => AddElement(new FixedEncodeType<string>.Byte(value));
    public AbiEncoder Int8(sbyte value) 
        => AddElement(new FixedEncodeType<string>.SByte(value));
    public AbiEncoder UInt16(ushort value) 
        => AddElement(new FixedEncodeType<string>.UShort(value));
    public AbiEncoder Int16(short value) 
        => AddElement(new FixedEncodeType<string>.Short(value));

    public AbiEncoder String(string value) 
        => AddElement(new DynamicEncodeType<string>.String(value));
    public AbiEncoder Bytes(byte[] arr) 
        => AddElement(new DynamicEncodeType<string>.Bytes(arr));

    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func) 
        => AddElement(new DynamicEncodeType<string>.Array(func(new AbiEncoder())));
    public AbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func) 
        => AddElement(new DynamicEncodeType<string>.Struct(typeId, func(new AbiEncoder())));

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
                case IDynamicEncodeType dynEncType:
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
                case IFixedEncodeType fixEncType:
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