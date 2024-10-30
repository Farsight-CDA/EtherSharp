using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI;

public partial class AbiEncoder : IAbiEncoder, IArrayAbiEncoder, IStructAbiEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private void AddElement(IEncodeType item)
    {
        if(item is IDynamicEncodeType dyn)
        {
            _payloadSize += dyn.PayloadSize;
        }

        _metadataSize += item.MetadataSize;
        _entries.Add(item);
    }

    uint IArrayAbiEncoder.MetadataSize => _metadataSize;
    uint IArrayAbiEncoder.PayloadSize => _payloadSize;

    uint IStructAbiEncoder.MetadataSize => _metadataSize;

    uint IStructAbiEncoder.PayloadSize => _payloadSize;

    private uint _payloadSize = 0;
    private uint _metadataSize = 0;

    public uint Size => _payloadSize + _metadataSize;

    int IAbiEncoder.Size => throw new NotImplementedException();

    public AbiEncoder UInt8(byte value)
    {
        AddElement(new FixedEncodeType<string>.Byte(value));
        return this;
    }

    public AbiEncoder Int8(sbyte value)
    {
        AddElement(new FixedEncodeType<string>.SByte(value));
        return this;
    }
    public AbiEncoder UInt16(ushort value)
    {

        AddElement(new FixedEncodeType<string>.UShort(value));
        return this;
    }

    public AbiEncoder Int16(short value)
    {

        AddElement(new FixedEncodeType<string>.Short(value));
        return this;
    }

    public AbiEncoder String(string value)
    {
        AddElement(new DynamicEncodeType<string>.String(value));
        return this;
    }

    public AbiEncoder Bytes(byte[] arr)
    {
        AddElement(new DynamicEncodeType<string>.Bytes(arr));
        return this;
    }

    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
    {

        AddElement(new DynamicEncodeType<string>.Array(func(new AbiEncoder())));
        return this;
    }

    public AbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
    {

        AddElement(new DynamicEncodeType<string>.Struct(typeId, func(new AbiEncoder())));
        return this;
    }

    IArrayAbiEncoder IArrayAbiEncoder.Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
        => Array(func);

    public void Build(Span<byte> result)
    {
        uint metadataOffset = 0;
        uint payloadOffset = _metadataSize;

        foreach(var entry in _entries)
        {
            if(entry is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) dynamicEncodeType.MetadataSize),
                    result.Slice(
                        (int) payloadOffset,
                        (int) dynamicEncodeType.PayloadSize),
                    payloadOffset
                );

                metadataOffset += dynamicEncodeType.MetadataSize;
                payloadOffset += dynamicEncodeType.PayloadSize;
            }
            else if(entry is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) entry.MetadataSize));
                metadataOffset += entry.MetadataSize;
            }
            else
            {
                throw new InvalidDataException(entry.GetType().FullName);
            }
        }

        _entries.Clear();
    }

    void IArrayAbiEncoder.WriteToParent(Span<byte> result, Span<byte> payload, uint payloadOffset)
    {
        uint metadataOffset = 0;

        foreach(var entry in _entries)
        {
            if(entry is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) dynamicEncodeType.MetadataSize),
                    result.Slice(
                        (int) payloadOffset,
                        (int) dynamicEncodeType.PayloadSize),
                    payloadOffset
                );
                metadataOffset += entry.MetadataSize;
                payloadOffset += dynamicEncodeType.PayloadSize;
            }
            else if(entry is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) entry.MetadataSize));
                metadataOffset += entry.MetadataSize;
            }
            else
            {
                throw new InvalidDataException(entry.GetType().FullName);
            }
        }
    }

    IStructAbiEncoder IStructAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
    => Struct(typeId, func);

    public void WriteToParent(Span<byte> result, Span<byte> payload, uint payloadOffset)
    {
        uint metadataOffset = 0;

        foreach(var entry in _entries)
        {
            if(entry is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) dynamicEncodeType.MetadataSize),
                    result.Slice(
                        (int) payloadOffset,
                        (int) dynamicEncodeType.PayloadSize),
                    payloadOffset
                );
                metadataOffset += entry.MetadataSize;
                payloadOffset += dynamicEncodeType.PayloadSize;
            }
            else if(entry is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) entry.MetadataSize));
                metadataOffset += entry.MetadataSize;
            }
            else
            {
                throw new InvalidDataException(entry.GetType().FullName);
            }
        }
    }

    public void WriteToParent(Span<byte> result, Span<byte> payload, int payloadOffset) => throw new NotImplementedException();
    IArrayAbiEncoder IArrayAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => Struct(typeId, func);
}