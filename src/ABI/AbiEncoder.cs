﻿namespace EtherSharp.ABI;

public partial class AbiEncoder : IArrayAbiEncoder
{

    private readonly List<IEncodeType> _entries = [];

    private void AddElement(IEncodeType item)
    {
        _payloadSize += item.PayloadSize;
        _metadataSize += item.MetadataSize;
        _entries.Add(item);
    }

    uint IArrayAbiEncoder.MetadataSize => _metadataSize;
    uint IArrayAbiEncoder.PayloadSize => _payloadSize;

    private uint _payloadSize = 0;
    private uint _metadataSize = 0;

    public uint Size => _payloadSize + _metadataSize;

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
        uint metadataOffset = 0;
        uint payloadOffset = _metadataSize;

        for(int i = 0; i < _entries.Count; i++)
        {
            if(_entries[i] is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset, 
                        (int) _entries[i].MetadataSize), 
                    result.Slice(
                        (int) payloadOffset, 
                        (int) _entries[i].PayloadSize), payloadOffset);
                metadataOffset += _entries[i].MetadataSize;
                payloadOffset += _entries[i].PayloadSize;
            }
            else if(_entries[i] is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset, 
                        (int) _entries[i].MetadataSize));
                metadataOffset += _entries[i].MetadataSize;
            }
            else
            {
                throw new InvalidDataException(_entries[i].GetType().FullName);
            }
        }

        _entries.Clear();
    }

    void IArrayAbiEncoder.WriteToParent(Span<byte> result, Span<byte> payload, uint payloadOffset)
    {
        uint metadataOffset = 0;

        for(int i = 0; i < _entries.Count; i++)
        {
            if(_entries[i] is IDynamicEncodeType dynamicEncodeType)
            {
                dynamicEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset, 
                        (int) _entries[i].MetadataSize), 
                    result.Slice(
                        (int) payloadOffset,
                        (int) _entries[i].PayloadSize), payloadOffset
                );
                metadataOffset += _entries[i].MetadataSize;
                payloadOffset += _entries[i].PayloadSize;
            }
            else if(_entries[i] is IFixedEncodeType fixedEncodeType)
            {
                fixedEncodeType.Encode(
                    result.Slice(
                        (int) metadataOffset,
                        (int) _entries[i].MetadataSize));
                metadataOffset += _entries[i].MetadataSize;
            }
            else
            {
                throw new InvalidDataException(_entries[i].GetType().FullName);
            }
        }
    }
}