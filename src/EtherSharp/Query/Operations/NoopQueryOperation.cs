namespace EtherSharp.Query.Operations;

internal sealed class NoopQueryOperation<T>(T value) : IQuery<T>
{
    private readonly T _value = value;

    int IQuery<T>.OperationCount => 0;

    void IQuery<T>.AddTo(QueryPlan plan) { }
    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => _value;
}
