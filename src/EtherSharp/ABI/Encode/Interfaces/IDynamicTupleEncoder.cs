namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IDynamicTupleEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    internal bool TryWritoTo(Span<byte> outputBuffer);

    public AbiEncoder String(string value);
    public AbiEncoder Bytes(byte[] arr);
    public AbiEncoder Array(Action<IArrayAbiEncoder> func);

    public IDynamicTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);
    public IDynamicTupleEncoder DynamicTuple(Action<IDynamicTupleEncoder> func);
}
