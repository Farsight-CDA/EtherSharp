namespace EtherSharp.Query.Operations;

internal sealed class NoopQueryOperation<T>(T value) : IQuery<T>
{
    private readonly T _value = value;

    int IQuery.OperationCount => 0;

    void IQuery.AddTo(IQueryPlan plan) { }
    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => _value;
}
