namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IArrayAbiEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public void Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func);
    public void DynamicTuple(Action<IDynamicTupleEncoder> func);
    public void FixedTuple(Action<IFixedTupleEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
