using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.ABI.Types.Interfaces;
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
            > 64 and <= 256 => new AbiTypes.BigInteger(
                number is BigInteger s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}"),
                isUnsigned, bitLength / 8),
            _ => throw new NotImplementedException()
        });
    }
    public AbiEncoder Bool(bool value)
        => AddElement(new AbiTypes.Bool(value));
    public AbiEncoder Address(string value)
        => AddElement(new AbiTypes.Address(value));

    public AbiEncoder String(string value)
        => AddElement(new AbiTypes.String(value));
    public AbiEncoder Bytes(byte[] arr)
        => AddElement(new AbiTypes.Bytes(arr));

    public AbiEncoder AddressArray(params string[] addresses)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Address>(
            addresses.Select(x => new AbiTypes.Address(x)).ToArray()));
    public AbiEncoder AddressArray(params Address[] addresses)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Address>(
            addresses.Select(x => new AbiTypes.Address(x.String)).ToArray()));
    public AbiEncoder StringArray(params string[] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.String>(
            value.Select(x => new AbiTypes.String(x)).ToArray()));
    public AbiEncoder BytesArray(params byte[][] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Bytes>(
            value.Select(x => new AbiTypes.Bytes(x)).ToArray()));

    public AbiEncoder Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func)
    {
        var encoder = new AbiEncoder();

        foreach(var value in values)
        {
            func(encoder, value);
        }

        return AddElement(new AbiTypes.Array(encoder));
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
                ? new AbiTypes.PrimitiveNumberArray<byte>(
                    numbers is byte[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"),
                    bitLength)
                : new AbiTypes.PrimitiveNumberArray<sbyte>(
                    numbers is sbyte[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}"),
                    bitLength),
            16 => isUnsigned
                ? new AbiTypes.PrimitiveNumberArray<ushort>(
                    numbers is ushort[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"),
                    bitLength)
                : new AbiTypes.PrimitiveNumberArray<short>(
                    numbers is short[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}"),
                    bitLength),
            > 16 and <= 32 => isUnsigned
                ? new AbiTypes.PrimitiveNumberArray<uint>(
                    numbers is uint[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"),
                    bitLength)
                : new AbiTypes.PrimitiveNumberArray<int>(
                    numbers is int[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"),
                    bitLength),
            > 32 and <= 64 => isUnsigned
                ? new AbiTypes.PrimitiveNumberArray<ulong>(
                    numbers is ulong[] us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"),
                    bitLength)
                : new AbiTypes.PrimitiveNumberArray<long>(
                    numbers is long[] s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"),
                    bitLength),
            > 64 and <= 256 => new AbiTypes.BigIntegerArray(
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
                            (int) fixEncType.Size
                    ));
                    metadataOffset += fixEncType.Size;
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