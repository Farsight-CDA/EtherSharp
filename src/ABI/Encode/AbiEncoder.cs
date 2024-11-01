﻿using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Encode;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Fixed;
using System.Numerics;

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
        if(item is IDynamicType dyn)
        {
            _payloadSize += dyn.PayloadSize;
        }

        _metadataSize += 32;
        _entries.Add(item);
        return this;
    }

    public AbiEncoder Number<TNumber>(TNumber number, bool isUnsigned, int bitLength)
    {
        if (bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        return AddElement(bitLength switch
        {
            8 => isUnsigned 
                ? new FixedType<object>.Byte(
                    number is byte us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"))
                : new FixedType<object>.SByte(
                    number is sbyte s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")),
            16 => isUnsigned
                ? new FixedType<object>.UShort(
                    number is ushort us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"))
                : new FixedType<object>.Short(
                    number is short s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")),
            > 16 and <= 32 => isUnsigned
                ? new FixedType<object>.UInt(
                    number is uint us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), bitLength)
                : new FixedType<object>.Int(
                    number is int s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), bitLength),
            > 32 and <= 64 => isUnsigned
                ? new FixedType<object>.ULong(
                    number is ulong us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), bitLength)
                : new FixedType<object>.Long(
                    number is long s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), bitLength),
            > 64 and <= 256 => new FixedType<object>.BigInteger(
                number is BigInteger s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"), 
                isUnsigned, bitLength),
            _ => throw new NotImplementedException()
        });
    }

    public AbiEncoder NumberArray<TNumber>(bool isUnsigned, int bitLength, params TNumber[] numbers)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        return AddElement(bitLength switch
        {
            8 => isUnsigned
                ? new DynamicType<object>.PrimitiveNumberArray<byte>(
                    numbers is byte[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"),
                    bitLength)
                : new DynamicType<object>.PrimitiveNumberArray<sbyte>(
                    numbers is sbyte[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}"),
                    bitLength),
            16 => isUnsigned
                ? new DynamicType<object>.PrimitiveNumberArray<ushort>(
                    numbers is ushort[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"),
                    bitLength)
                : new DynamicType<object>.PrimitiveNumberArray<short>(
                    numbers is short[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}"),
                    bitLength),
            > 16 and <= 32 => isUnsigned
                ? new DynamicType<object>.PrimitiveNumberArray<uint>(
                    numbers is uint[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), 
                    bitLength)
                : new DynamicType<object>.PrimitiveNumberArray<int>(
                    numbers is int[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), 
                    bitLength),
            > 32 and <= 64 => isUnsigned
                ? new DynamicType<object>.PrimitiveNumberArray<ulong>(
                    numbers is ulong[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), 
                    bitLength)
                : new DynamicType<object>.PrimitiveNumberArray<long>(
                    numbers is long[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), 
                    bitLength),
            > 64 and <= 256 => new DynamicType<object>.BigIntegerArray(
                numbers is BigInteger[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"),
                isUnsigned, bitLength),
            _ => throw new NotImplementedException()
        });
    }

    public AbiEncoder UInt8(byte value)
        => AddElement(new FixedType<string>.Byte(value));
    public AbiEncoder Int8(sbyte value)
        => AddElement(new FixedType<string>.SByte(value));
    public AbiEncoder UInt16(ushort value)
        => AddElement(new FixedType<string>.UShort(value));
    public AbiEncoder Int16(short value)
        => AddElement(new FixedType<string>.Short(value));

    public AbiEncoder String(string value)
        => AddElement(new DynamicType<string>.String(value));
    public AbiEncoder Bytes(byte[] arr)
        => AddElement(new DynamicType<string>.Bytes(arr));

    public AbiEncoder StringArray(params string[] value)
        => AddElement(new DynamicType<string>.EncodeTypeArray<DynamicType<string>.String>(
            value.Select(x => new DynamicType<string>.String(x)).ToArray()));
    public AbiEncoder BytesArray(params byte[][] value)
        => AddElement(new DynamicType<string>.EncodeTypeArray<DynamicType<string>.Bytes>(
            value.Select(x => new DynamicType<string>.Bytes(x)).ToArray()));

    public AbiEncoder Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func) 
        => AddElement(new DynamicType<string>.Array(func(new AbiEncoder())));
    public AbiEncoder Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => AddElement(new DynamicType<string>.Struct(typeId, func(new AbiEncoder())));

    IArrayAbiEncoder IArrayAbiEncoder.Array(Func<IArrayAbiEncoder, IArrayAbiEncoder> func)
        => Array(func);
    IArrayAbiEncoder IArrayAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => Struct(typeId, func);
    IStructAbiEncoder IStructAbiEncoder.Struct(uint typeId, Func<IStructAbiEncoder, IStructAbiEncoder> func)
        => Struct(typeId, func);

    public void WritoTo(Span<byte> result)
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

    public byte[] Build()
    {
        byte[] buffer = new byte[Size];
        WritoTo(buffer);
        return buffer;
    }
}