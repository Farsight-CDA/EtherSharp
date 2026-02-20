using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.ABI.Types.Base;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.ABI;

public partial class AbiEncoder : IArrayAbiEncoder, IFixedTupleEncoder, IDynamicTupleEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private int _payloadSize;
    private int _metadataSize;

    public int Size => _payloadSize + _metadataSize;
    int IArrayAbiEncoder.MetadataSize => _metadataSize;
    int IArrayAbiEncoder.PayloadSize => _payloadSize;
    int IFixedTupleEncoder.MetadataSize => _metadataSize;
    int IFixedTupleEncoder.PayloadSize => _payloadSize;
    int IDynamicTupleEncoder.MetadataSize => _metadataSize;
    int IDynamicTupleEncoder.PayloadSize => _payloadSize;

    private AbiEncoder AddElement(IEncodeType item)
    {
        if(item is IDynamicType dyn)
        {
            _payloadSize += dyn.PayloadSize;
            _metadataSize += 32;
        }
        else if(item is IFixedType fix)
        {
            _metadataSize += fix.Size;
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
                ? new AbiTypes.Byte(
                    number is byte us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"))
                : new AbiTypes.SByte(
                    number is sbyte s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")),
            16 => isUnsigned
                ? new AbiTypes.UShort(
                    number is ushort us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"))
                : new AbiTypes.Short(
                    number is short s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")),
            > 16 and <= 32 => isUnsigned
                ? new AbiTypes.UInt(
                    number is uint us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), bitLength / 8)
                : new AbiTypes.Int(
                    number is int s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), bitLength / 8),
            > 32 and <= 64 => isUnsigned
                ? new AbiTypes.ULong(
                    number is ulong us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), bitLength / 8)
                : new AbiTypes.Long(
                    number is long s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), bitLength / 8),
            > 64 and <= 256 => isUnsigned
                ? new AbiTypes.UInt256(
                    number is UInt256 us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(UInt256)}"), bitLength / 8)
                : new AbiTypes.Int256(
                    number is Int256 s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(Int256)}"), bitLength / 8),
            _ => throw new NotSupportedException()
        });
    }
    public AbiEncoder Bool(bool value)
        => AddElement(new AbiTypes.Bool(value));
    public AbiEncoder Address(Address value)
        => AddElement(new AbiTypes.Address(value));

    public AbiEncoder String(string value)
            => AddElement(new AbiTypes.String(value));
    public AbiEncoder Bytes(ReadOnlyMemory<byte> arr)
        => AddElement(new AbiTypes.Bytes(arr));

    public AbiEncoder BoolArray(params bool[] values)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Bool>(
                [.. values.Select(x => new AbiTypes.Bool(x))]));
    public AbiEncoder AddressArray(params Address[] addresses)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Address>(
            [.. addresses.Select(x => new AbiTypes.Address(x))]));
    public AbiEncoder StringArray(params string[] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.String>(
            [.. value.Select(x => new AbiTypes.String(x))]));
    public AbiEncoder BytesArray(params ReadOnlyMemory<byte>[] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Bytes>(
            [.. value.Select(x => new AbiTypes.Bytes(x))]));

    public AbiEncoder Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func)
    {
        var encoder = new AbiEncoder();
        uint elementCount = 0;

        foreach(var value in values)
        {
            func(encoder, value);
            elementCount++;
        }

        return AddElement(new AbiTypes.Array(encoder, elementCount));
    }

    void IArrayAbiEncoder.Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func)
        => Array(values, func);

    public AbiEncoder DynamicTuple(Action<IDynamicTupleEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new AbiTypes.DynamicTuple(encoder));
    }
    public AbiEncoder FixedTuple(Action<IFixedTupleEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new AbiTypes.FixedTuple(encoder));
    }

    void IArrayAbiEncoder.DynamicTuple(Action<IDynamicTupleEncoder> func)
        => DynamicTuple(func);
    void IArrayAbiEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);

    IFixedTupleEncoder IFixedTupleEncoder.Bool(bool value)
        => Bool(value);
    IFixedTupleEncoder IFixedTupleEncoder.Address(Address value)
        => Address(value);

    IDynamicTupleEncoder IDynamicTupleEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);
    IDynamicTupleEncoder IDynamicTupleEncoder.DynamicTuple(Action<IDynamicTupleEncoder> func)
        => DynamicTuple(func);
    IFixedTupleEncoder IFixedTupleEncoder.FixedTuple(Action<IFixedTupleEncoder> func)
        => FixedTuple(func);

    IDynamicTupleEncoder IDynamicTupleEncoder.Bool(bool value)
        => Bool(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Address(Address value)
        => Address(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.String(string value)
        => String(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes(ReadOnlyMemory<byte> arr)
            => Bytes(arr);
    IDynamicTupleEncoder IDynamicTupleEncoder.BoolArray(params bool[] values)
        => BoolArray(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.AddressArray(params Address[] addresses)
        => AddressArray(addresses);
    IDynamicTupleEncoder IDynamicTupleEncoder.StringArray(params string[] values)
        => StringArray(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.BytesArray(params ReadOnlyMemory<byte>[] values)
        => BytesArray(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func)
        => Array(values, func);

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
                ? new AbiTypes.SizedNumberArray<byte>(
                    numbers is byte[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"),
                    bitLength)
                : new AbiTypes.SizedNumberArray<sbyte>(
                    numbers is sbyte[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}"),
                    bitLength),
            16 => isUnsigned
                ? new AbiTypes.SizedNumberArray<ushort>(
                    numbers is ushort[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"),
                    bitLength)
                : new AbiTypes.SizedNumberArray<short>(
                    numbers is short[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}"),
                    bitLength),
            > 16 and <= 32 => isUnsigned
                ? new AbiTypes.SizedNumberArray<uint>(
                    numbers is uint[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"),
                    bitLength)
                : new AbiTypes.SizedNumberArray<int>(
                    numbers is int[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"),
                    bitLength),
            > 32 and <= 64 => isUnsigned
                ? new AbiTypes.SizedNumberArray<ulong>(
                    numbers is ulong[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"),
                    bitLength)
                : new AbiTypes.SizedNumberArray<long>(
                    numbers is long[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"),
                    bitLength),
            > 64 and <= 256 => isUnsigned
                ? new AbiTypes.UInt256Array(
                    numbers is UInt256[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(UInt256)}"),
                    bitLength)
                : new AbiTypes.Int256Array(
                    numbers is Int256[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(Int256)}"),
                    bitLength),
            _ => throw new NotSupportedException()
        });
    }

    public bool TryWritoTo(Span<byte> outputBuffer)
    {
        if(outputBuffer.Length < Size)
        {
            throw new ArgumentException("Output buffer too small", nameof(outputBuffer));
        }

        int metaDataOffset = 0;
        int payloadOffset = _metadataSize;

        foreach(var entry in _entries)
        {
            switch(entry)
            {
                case IDynamicType dynEncType:
                    dynEncType.Encode(
                        outputBuffer.Slice(
                            metaDataOffset,
                            32),
                        outputBuffer.Slice(
                            payloadOffset,
                            dynEncType.PayloadSize),
                        payloadOffset
                    );
                    metaDataOffset += 32;
                    payloadOffset += dynEncType.PayloadSize;
                    break;
                case IFixedType fixEncType:
                    fixEncType.Encode(
                        outputBuffer.Slice(
                            metaDataOffset,
                            fixEncType.Size
                    ));
                    metaDataOffset += fixEncType.Size;
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