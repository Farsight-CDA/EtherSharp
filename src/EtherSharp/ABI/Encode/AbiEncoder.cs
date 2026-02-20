using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.ABI.Types.Base;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.ABI;

/// <summary>
/// Builds standard (non-packed) Ethereum ABI-encoded payloads.
/// </summary>
public partial class AbiEncoder : IArrayAbiEncoder, IFixedTupleEncoder, IDynamicTupleEncoder
{
    private readonly List<IEncodeType> _entries = [];

    private int _payloadSize;
    private int _metadataSize;

    /// <summary>
    /// Gets the total encoded size in bytes.
    /// </summary>
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

    /// <summary>
    /// Encodes a numeric value with the given signedness and bit width.
    /// </summary>
    /// <typeparam name="TNumber">CLR number type matching the requested ABI width.</typeparam>
    /// <param name="number">Value to encode.</param>
    /// <param name="isUnsigned"><see langword="true"/> for uintN, <see langword="false"/> for intN.</param>
    /// <param name="bitLength">ABI bit width (8..256 in steps of 8).</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
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
    /// <summary>
    /// Encodes a boolean value.
    /// </summary>
    /// <param name="value">Boolean to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder Bool(bool value)
        => AddElement(new AbiTypes.Bool(value));

    /// <summary>
    /// Encodes an Ethereum address.
    /// </summary>
    /// <param name="value">Address to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder Address(Address value)
        => AddElement(new AbiTypes.Address(value));

    /// <summary>
    /// Encodes a UTF-8 string as a dynamic ABI value.
    /// </summary>
    /// <param name="value">String to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder String(string value)
            => AddElement(new AbiTypes.String(value));

    /// <summary>
    /// Encodes a dynamic byte sequence.
    /// </summary>
    /// <param name="arr">Bytes to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder Bytes(ReadOnlyMemory<byte> arr)
        => AddElement(new AbiTypes.Bytes(arr));

    /// <summary>
    /// Encodes a dynamic array of booleans.
    /// </summary>
    /// <param name="values">Values to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder BoolArray(params bool[] values)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Bool>(
                [.. values.Select(x => new AbiTypes.Bool(x))]));

    /// <summary>
    /// Encodes a dynamic array of addresses.
    /// </summary>
    /// <param name="addresses">Addresses to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder AddressArray(params Address[] addresses)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Address>(
            [.. addresses.Select(x => new AbiTypes.Address(x))]));

    /// <summary>
    /// Encodes a dynamic array of strings.
    /// </summary>
    /// <param name="value">Strings to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder StringArray(params string[] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.String>(
            [.. value.Select(x => new AbiTypes.String(x))]));

    /// <summary>
    /// Encodes a dynamic array of dynamic byte sequences.
    /// </summary>
    /// <param name="value">Byte sequences to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder BytesArray(params ReadOnlyMemory<byte>[] value)
        => AddElement(new AbiTypes.EncodeTypeArray<AbiTypes.Bytes>(
            [.. value.Select(x => new AbiTypes.Bytes(x))]));

    /// <summary>
    /// Encodes a dynamic array by encoding each element with the provided callback.
    /// </summary>
    /// <typeparam name="T">Input element type.</typeparam>
    /// <param name="values">Elements to encode.</param>
    /// <param name="func">Callback that writes one element into an element encoder.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
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

    /// <summary>
    /// Encodes a dynamic tuple element.
    /// </summary>
    /// <param name="func">Callback that writes tuple fields.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public AbiEncoder DynamicTuple(Action<IDynamicTupleEncoder> func)
    {
        var encoder = new AbiEncoder();
        func(encoder);
        return AddElement(new AbiTypes.DynamicTuple(encoder));
    }
    /// <summary>
    /// Encodes a fixed tuple element.
    /// </summary>
    /// <param name="func">Callback that writes tuple fields.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
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

    /// <summary>
    /// Encodes an array of numeric values with the given signedness and bit width.
    /// </summary>
    /// <typeparam name="TNumber">CLR number type matching the requested ABI width.</typeparam>
    /// <param name="isUnsigned"><see langword="true"/> for uintN[], <see langword="false"/> for intN[].</param>
    /// <param name="bitLength">ABI bit width (8..256 in steps of 8).</param>
    /// <param name="numbers">Values to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
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

    /// <summary>
    /// Writes the encoded payload into a caller-provided buffer.
    /// </summary>
    /// <param name="outputBuffer">Destination buffer. Must be at least <see cref="Size"/> bytes long.</param>
    /// <returns><see langword="true"/> when writing succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="outputBuffer"/> is too small.</exception>
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

    /// <summary>
    /// Materializes the current encoded payload as a new byte array.
    /// </summary>
    /// <returns>The encoded ABI payload.</returns>
    public byte[] Build()
    {
        byte[] buffer = new byte[Size];
        _ = TryWritoTo(buffer);
        return buffer;
    }
}
