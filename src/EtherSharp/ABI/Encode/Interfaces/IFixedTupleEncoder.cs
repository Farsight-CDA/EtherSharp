namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IFixedTupleEncoder
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }

    public IFixedTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
