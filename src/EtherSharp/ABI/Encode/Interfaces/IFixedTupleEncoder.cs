using EtherSharp.Types;

namespace EtherSharp.ABI.Encode.Interfaces;

public partial interface IFixedTupleEncoder
{
    public int MetadataSize { get; }
    public int PayloadSize { get; }

    public IFixedTupleEncoder Bool(bool value);
    public IFixedTupleEncoder Address(Address value);

    public IFixedTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);

    internal bool TryWritoTo(Span<byte> outputBuffer);
}
