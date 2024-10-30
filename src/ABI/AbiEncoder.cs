using EtherSharp.ABI;
using EVM.net.ABI;

public partial class AbiEncoder : IArrayAbiEncoder
{

    private readonly List<IEncodeType> _entries = [];

    private void AddElement(IEncodeType item)
    {
        _payloadSize += item.PayloadSize;
        _metadataSize += item.MetadataSize;
        _entries.Add(item);
    }

    int IArrayAbiEncoder.MetadataSize => _metadataSize;
    int IArrayAbiEncoder.PayloadSize => _payloadSize;

    private int _payloadSize = 0;

    private int _metadataSize = 0;

    public int Size => _payloadSize + _metadataSize;

    public AbiEncoder UInt8(byte value)
    {
        AddElement(new FixedEncodeType<string>.UInt8(value));
        return this;
    }

    public AbiEncoder Int8(sbyte value)
    {
        AddElement(new FixedEncodeType<string>.Int8(value));
        return this;
    }
    public AbiEncoder UInt16(ushort value)
    {

        AddElement(new FixedEncodeType<string>.UInt16(value));
        return this;
    }

    public AbiEncoder Int16(short value)
    {

        AddElement(new FixedEncodeType<string>.Int16(value));
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

        AddElement(new DynamicEncodeType<string>.AArray(func(new AbiEncoder())));
        return this;
    }

    IArrayAbiEncoder IArrayAbiEncoder.Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
        => Array(func);

    public void Build(Span<byte> result)
    {
        int metadataOffset = 0;
        int payloadOffset = _metadataSize;

        for(int i = 0; i < _entries.Count; i++)
        {
            if(_entries[i] is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(result.Slice(metadataOffset, _entries[i].MetadataSize), result.Slice(payloadOffset, _entries[i].PayloadSize), payloadOffset);
                metadataOffset += _entries[i].MetadataSize;
                payloadOffset += _entries[i].PayloadSize;
            }
            else if(_entries[i] is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(result.Slice(metadataOffset, _entries[i].MetadataSize));
                metadataOffset += _entries[i].MetadataSize;
            }
            else
            {
                throw new InvalidDataException(_entries[i].GetType().FullName);
            }
        }

        _entries.Clear();
    }

    void IArrayAbiEncoder.Build(Span<byte> result, Span<byte> payload, int payloadOffset)
    {
        int metadataOffset = 0;

        for(int i = 0; i < _entries.Count; i++)
        {
            if(_entries[i] is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(result.Slice(metadataOffset, _entries[i].MetadataSize), result.Slice(payloadOffset, _entries[i].PayloadSize), payloadOffset);
                metadataOffset += _entries[i].MetadataSize;
                payloadOffset += _entries[i].PayloadSize;
            }
            else if(_entries[i] is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(result.Slice(metadataOffset, _entries[i].MetadataSize));
                metadataOffset += _entries[i].MetadataSize;
            }
            else
            {
                throw new InvalidDataException(_entries[i].GetType().FullName);
            }
        }
    }
}