using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Encode;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Fixed;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.ABI;

public partial class AbiEncoder : IArrayAbiEncoder, IFixedTupleEncoder, IDynamicTupleEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private uint _payloadSize;
    private uint _metadataSize;

    public int Size => (int) (_payloadSize + _metadataSize);
    uint IArrayAbiEncoder.MetadataSize => _metadataSize;
    uint IArrayAbiEncoder.PayloadSize => _payloadSize;
    uint IFixedTupleEncoder.MetadataSize => _metadataSize;
    uint IFixedTupleEncoder.PayloadSize => _payloadSize;
    uint IDynamicTupleEncoder.MetadataSize => _metadataSize;
    uint IDynamicTupleEncoder.PayloadSize => _payloadSize;

    private AbiEncoder AddElement(IEncodeType item)
    {
        if(item is IDynamicType dyn)
        {
            _payloadSize += dyn.PayloadSize;
            _metadataSize += 32;
        }
        else if(item is IFixedType fix)
        {
            _metadataSize += fix.MetadataSize;
        }
        else
        {
            throw new NotSupportedException();
        }

        _entries.Add(item);
        return this;
    }

    public AbiEncoder Number<TNumber>(TNumber number, bool isUnsigned, int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        return AddElement(bitLength switch
        {
            8 => isUnsigned
                ? new FixedType.Byte(
                    number is byte us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"))
                : new FixedType.SByte(
                    number is sbyte s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")),
            16 => isUnsigned
                ? new FixedType.UShort(
                    number is ushort us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"))
                : new FixedType.Short(
                    number is short s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")),
            > 16 and <= 32 => isUnsigned
                ? new FixedType.UInt(
                    number is uint us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), bitLength)
                : new FixedType.Int(
                    number is int s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), bitLength),
            > 32 and <= 64 => isUnsigned
                ? new FixedType.ULong(
                    number is ulong us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), bitLength)
                : new FixedType.Long(
                    number is long s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), bitLength),
            > 64 and <= 256 => new FixedType.BigInteger(
                number is BigInteger s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"),
                isUnsigned, bitLength),
            _ => throw new NotImplementedException()
        });
    }
    public AbiEncoder Bool(bool value)
        => AddElement(new FixedType.Bool(value));
    public AbiEncoder Address(string value)
        => AddElement(new FixedType.Address(value));

    public AbiEncoder String(string value)
        => AddElement(new DynamicType.String(value));
    public AbiEncoder Bytes(byte[] arr)
        => AddElement(new DynamicType.Bytes(arr));

    public AbiEncoder AddressArray(params string[] addresses)
        => AddElement(new DynamicType.EncodeTypeArray<FixedType.Address>(
            addresses.Select(x => new FixedType.Address(x)).ToArray()));
    public AbiEncoder AddressArray(params Address[] addresses)
        => AddElement(new DynamicType.EncodeTypeArray<FixedType.Address>(
            addresses.Select(x => new FixedType.Address(x.String)).ToArray()));
    public AbiEncoder StringArray(params string[] value)
        => AddElement(new DynamicType.EncodeTypeArray<DynamicType.String>(
            value.Select(x => new DynamicType.String(x)).ToArray()));
    public AbiEncoder BytesArray(params byte[][] value)
        => AddElement(new DynamicType.EncodeTypeArray<DynamicType.Bytes>(
            value.Select(x => new DynamicType.Bytes(x)).ToArray()));

    public AbiEncoder Array(Action<IArrayAbiEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new DynamicType.Array(encoder));
    }

    IArrayAbiEncoder IArrayAbiEncoder.Array(Action<IArrayAbiEncoder> func)
        => Array(func);

    public AbiEncoder DynamicTuple(Action<IDynamicTupleEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new DynamicType.Tuple(encoder));
    }
    public AbiEncoder FixedTuple(Action<IFixedTupleEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new FixedType.Tuple(encoder));
    }

    IArrayAbiEncoder IArrayAbiEncoder.DynamicTuple(Action<IDynamicTupleEncoder> func)
        => DynamicTuple(func);
    IArrayAbiEncoder IArrayAbiEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);

    IDynamicTupleEncoder IDynamicTupleEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);
    IDynamicTupleEncoder IDynamicTupleEncoder.DynamicTuple(Action<IDynamicTupleEncoder> func)
        => DynamicTuple(func);
    IFixedTupleEncoder IFixedTupleEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);

    IDynamicTupleEncoder IDynamicTupleEncoder.String(string value)
        => String(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes(byte[] arr)
        => Bytes(arr);
    IDynamicTupleEncoder IDynamicTupleEncoder.AddressArray(params string[] addresses)
        => AddressArray(addresses);
    IDynamicTupleEncoder IDynamicTupleEncoder.StringArray(params string[] values)
        => StringArray(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.BytesArray(params byte[][] values)
        => BytesArray(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Array(Action<IArrayAbiEncoder> func)
        => Array(func);

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
                ? new DynamicType.PrimitiveNumberArray<byte>(
                    numbers is byte[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"),
                    bitLength)
                : new DynamicType.PrimitiveNumberArray<sbyte>(
                    numbers is sbyte[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}"),
                    bitLength),
            16 => isUnsigned
                ? new DynamicType.PrimitiveNumberArray<ushort>(
                    numbers is ushort[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"),
                    bitLength)
                : new DynamicType.PrimitiveNumberArray<short>(
                    numbers is short[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}"),
                    bitLength),
            > 16 and <= 32 => isUnsigned
                ? new DynamicType.PrimitiveNumberArray<uint>(
                    numbers is uint[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"),
                    bitLength)
                : new DynamicType.PrimitiveNumberArray<int>(
                    numbers is int[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"),
                    bitLength),
            > 32 and <= 64 => isUnsigned
                ? new DynamicType.PrimitiveNumberArray<ulong>(
                    numbers is ulong[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"),
                    bitLength)
                : new DynamicType.PrimitiveNumberArray<long>(
                    numbers is long[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"),
                    bitLength),
            > 64 and <= 256 => new DynamicType.BigIntegerArray(
                numbers is BigInteger[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"),
                isUnsigned, bitLength),
            _ => throw new NotImplementedException()
        });
    }

    public bool TryWritoTo(Span<byte> outputBuffer)
    {
        if(outputBuffer.Length < Size)
        {
            throw new ArgumentException("Output buffer too small", nameof(outputBuffer));
        }

        uint metadataOffset = 0;
        uint payloadOffset = _metadataSize;

        foreach(var entry in _entries)
        {
            switch(entry)
            {
                case IDynamicType dynEncType:
                    dynEncType.Encode(
                        outputBuffer.Slice(
                            (int) metadataOffset,
                            32),
                        outputBuffer.Slice(
                            (int) payloadOffset,
                            (int) dynEncType.PayloadSize),
                        payloadOffset
                    );
                    metadataOffset += 32;
                    payloadOffset += dynEncType.PayloadSize;
                    break;
                case IFixedType fixEncType:
                    fixEncType.Encode(
                        outputBuffer.Slice(
                            (int) metadataOffset,
                            (int) fixEncType.MetadataSize
                    ));
                    metadataOffset += fixEncType.MetadataSize;
                    break;
                default:
                    throw new NotImplementedException(entry.GetType().FullName);
            }
        }

        return true;
    }

    public byte[] Build()
    {
        byte[] buffer = new byte[Size];
        _ = TryWritoTo(buffer);
        return buffer;
    }
}