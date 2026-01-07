using EtherSharp.Numerics;

namespace EtherSharp.Query.Operations;

internal class NoopQueryOperation<T>(T value) : IQuery, IQuery<T>
{
    private readonly T _value = value;

    public int CallDataLength => 0;
    public UInt256 EthValue => 0;
    IReadOnlyList<IQuery> IQuery<T>.Queries => [];

    public void Encode(Span<byte> buffer) { }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 0;
    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults)
        => _value;
}
