using EtherSharp.Types;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IDynamicTupleEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    internal bool TryWritoTo(Span<byte> outputBuffer);

    public IDynamicTupleEncoder Bool(bool value);
    public IDynamicTupleEncoder Address(Address value);

    public IDynamicTupleEncoder String(string value);
    public IDynamicTupleEncoder Bytes(byte[] value);

    public IDynamicTupleEncoder BoolArray(params bool[] values);
    public IDynamicTupleEncoder AddressArray(params Address[] addresses);
    public IDynamicTupleEncoder StringArray(params string[] values);
    public IDynamicTupleEncoder BytesArray(params byte[][] values);

    public IDynamicTupleEncoder Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func);

    public IDynamicTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);
    public IDynamicTupleEncoder DynamicTuple(Action<IDynamicTupleEncoder> func);
}
