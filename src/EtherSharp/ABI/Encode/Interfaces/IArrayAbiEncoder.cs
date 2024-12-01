namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IArrayAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public IArrayAbiEncoder Array(Action<IArrayAbiEncoder> func);
    public IArrayAbiEncoder DynamicTuple(Action<IDynamicTupleEncoder> func);
    public IArrayAbiEncoder FixedTuple(Action<IFixedTupleEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
